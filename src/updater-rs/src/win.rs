#![allow(non_snake_case)]

use std::env;
use std::ffi::OsStr;
use std::os::windows::ffi::OsStrExt;
use std::ptr;

type HANDLE = *mut std::ffi::c_void;
type HINSTANCE = *mut std::ffi::c_void;
type HWND = *mut std::ffi::c_void;
type LPCWSTR = *const u16;
type DWORD = u32;
type BOOL = i32;
type INT = i32;

const PROCESS_SYNCHRONIZE: DWORD = 0x00100000;
const WAIT_TIMEOUT: DWORD = 0x00000102;
const SW_SHOWNORMAL: INT = 1;

#[link(name = "kernel32")]
extern "system" {
    fn OpenProcess(dwDesiredAccess: DWORD, bInheritHandle: BOOL, dwProcessId: DWORD) -> HANDLE;
    fn WaitForSingleObject(hHandle: HANDLE, dwMilliseconds: DWORD) -> DWORD;
    fn CloseHandle(hObject: HANDLE) -> BOOL;
}

#[link(name = "shell32")]
extern "system" {
    fn ShellExecuteW(
        hwnd: HWND,
        lpOperation: LPCWSTR,
        lpFile: LPCWSTR,
        lpParameters: LPCWSTR,
        lpDirectory: LPCWSTR,
        nShowCmd: INT,
    ) -> HINSTANCE;
}

pub fn is_process_alive(pid: u32) -> bool {
    unsafe {
        let handle = OpenProcess(PROCESS_SYNCHRONIZE, 0, pid);
        if handle.is_null() {
            return false;
        }
        let result = WaitForSingleObject(handle, 0);
        CloseHandle(handle);
        result == WAIT_TIMEOUT
    }
}

pub fn relaunch_elevated() -> bool {
    let exe = env::current_exe().unwrap_or_default();
    let args: Vec<String> = env::args().skip(1).collect();
    let args_str = args
        .iter()
        .map(|a| {
            if a.contains(' ') {
                format!("\"{}\"", a)
            } else {
                a.clone()
            }
        })
        .collect::<Vec<_>>()
        .join(" ");

    let verb = to_wide("runas");
    let file = to_wide(exe.to_str().unwrap_or("updater.exe"));
    let params = to_wide(&args_str);

    unsafe {
        let result = ShellExecuteW(
            ptr::null_mut(),
            verb.as_ptr(),
            file.as_ptr(),
            params.as_ptr(),
            ptr::null(),
            SW_SHOWNORMAL,
        );
        (result as usize) > 32
    }
}

fn to_wide(s: &str) -> Vec<u16> {
    OsStr::new(s).encode_wide().chain(std::iter::once(0)).collect()
}
