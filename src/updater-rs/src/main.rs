#![windows_subsystem = "windows"]

use std::env;
use std::fs;
use std::io::{self, Write};
use std::path::{Path, PathBuf};
use std::process;
use std::thread;
use std::time::{Duration, Instant};

mod win;

fn main() {
    let log_path = env::current_exe()
        .unwrap_or_default()
        .parent()
        .and_then(|p| p.parent())
        .unwrap_or(Path::new("."))
        .join("updater.log");
    let mut log = fs::OpenOptions::new()
        .create(true)
        .write(true)
        .truncate(true)
        .open(&log_path)
        .ok();

    macro_rules! log {
        ($($arg:tt)*) => {
            let msg = format!($($arg)*);
            eprintln!("{}", msg);
            if let Some(ref mut f) = log {
                let _ = writeln!(f, "{}", msg);
                let _ = f.flush();
            }
        };
    }

    log!("[updater] started, args: {:?}", env::args().collect::<Vec<_>>());

    let args = match Args::parse() {
        Ok(a) => a,
        Err(e) => {
            log!("[updater] invalid arguments: {}", e);
            process::exit(1);
        }
    };

    log!("[updater] pid={}, source={}, target={}, exe={}",
        args.pid, args.source.display(), args.target.display(), args.exe);

    if args.pid > 0 {
        log!("[updater] waiting for process {} to exit...", args.pid);
        let timeout = Duration::from_secs(30);
        let start = Instant::now();

        while start.elapsed() < timeout {
            if !win::is_process_alive(args.pid) {
                break;
            }
            thread::sleep(Duration::from_millis(200));
        }

        if win::is_process_alive(args.pid) {
            log!("[updater] process {} did not exit within 30s, proceeding", args.pid);
        } else {
            log!("[updater] process {} exited", args.pid);
        }
    }

    thread::sleep(Duration::from_millis(500));

    let test_file = args.target.join(".update_test");
    match fs::write(&test_file, b"test") {
        Ok(_) => {
            let _ = fs::remove_file(&test_file);
        }
        Err(e) => {
            let _ = fs::remove_file(&test_file);
            if e.kind() == io::ErrorKind::PermissionDenied {
                log!("[updater] no write permission, requesting elevation...");
                if win::relaunch_elevated() {
                    process::exit(0);
                } else {
                    log!("[updater] elevation failed");
                    process::exit(2);
                }
            } else {
                log!("[updater] write test failed: {}", e);
                process::exit(2);
            }
        }
    }

    log!("[updater] copying files...");
    match copy_directory(&args.source, &args.target, &mut log, true) {
        Ok(count) => {
            log!("[updater] copied {} files", count);
        }
        Err(e) => {
            log!("[updater] FATAL: copy failed: {}", e);
            process::exit(3);
        }
    }

    // Close log before deleting cache dir
    drop(log);

    if let Some(cache_dir) = args.source.parent() {
        let _ = fs::remove_dir_all(cache_dir);
    }

    let exe_path = args.target.join(&args.exe);
    if exe_path.exists() {
        eprintln!("[updater] launching {}", exe_path.display());
        let _ = process::Command::new(&exe_path)
            .current_dir(&args.target)
            .spawn();
    }

    eprintln!("[updater] done");
}

fn copy_directory(src: &Path, dst: &Path, log: &mut Option<fs::File>, skip_self: bool) -> io::Result<usize> {
    let mut count = 0;
    let self_exe = if skip_self {
        env::current_exe().ok()
    } else {
        None
    };

    for entry in fs::read_dir(src)? {
        let entry = entry?;
        let file_type = entry.file_type()?;
        let src_path = entry.path();
        let relative = src_path.strip_prefix(src).unwrap();
        let dst_path = dst.join(relative);

        if file_type.is_dir() {
            fs::create_dir_all(&dst_path)?;
            count += copy_directory(&src_path, &dst_path, log, false)?;
        } else {
            // Skip the running updater itself and its log
            if let Some(ref exe) = self_exe {
                if src_path == *exe {
                    continue;
                }
            }
            let file_name = src_path.file_name().unwrap_or_default();
            if file_name == "updater.log" {
                continue;
            }

            if let Some(parent) = dst_path.parent() {
                fs::create_dir_all(parent)?;
            }

            let mut last_err = None;
            for attempt in 0..5 {
                match fs::copy(&src_path, &dst_path) {
                    Ok(_) => {
                        last_err = None;
                        break;
                    }
                    Err(e) => {
                        if let Some(ref mut f) = log {
                            let _ = writeln!(f, "[updater] retry {}/5 for {}: {}",
                                attempt + 1, relative.display(), e);
                        }
                        last_err = Some(e);
                        thread::sleep(Duration::from_millis(500));
                    }
                }
            }

            if let Some(e) = last_err {
                return Err(e);
            }
            count += 1;
        }
    }

    Ok(count)
}

struct Args {
    pid: u32,
    source: PathBuf,
    target: PathBuf,
    exe: String,
}

impl Args {
    fn parse() -> Result<Self, String> {
        let args: Vec<String> = env::args().collect();
        let mut pid = 0u32;
        let mut source = String::new();
        let mut target = String::new();
        let mut exe = String::new();

        let mut i = 1;
        while i < args.len() {
            match args[i].as_str() {
                "--pid" => {
                    i += 1;
                    pid = args.get(i).ok_or("missing --pid value")?
                        .parse().map_err(|e| format!("invalid --pid: {}", e))?;
                }
                "--source" => {
                    i += 1;
                    source = args.get(i).ok_or("missing --source value")?.clone();
                }
                "--target" => {
                    i += 1;
                    target = args.get(i).ok_or("missing --target value")?.clone();
                }
                "--exe" => {
                    i += 1;
                    exe = args.get(i).ok_or("missing --exe value")?.clone();
                }
                _ => {}
            }
            i += 1;
        }

        if source.is_empty() || target.is_empty() || exe.is_empty() {
            return Err("missing required arguments: --source, --target, --exe".into());
        }

        Ok(Self {
            pid,
            source: PathBuf::from(source),
            target: PathBuf::from(target),
            exe,
        })
    }
}
