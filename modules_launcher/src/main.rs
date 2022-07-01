#![windows_subsystem = "windows"]


use std::{env, fs, process};

fn main() {
    let s = fs::read_to_string(env::var("APPDATA").unwrap() + "/RemoveDislike/Modules/Launcher.config").unwrap();
    s.split('\n').filter(|x| !x.starts_with("//") && !x.starts_with('#')).for_each(|x| {
        let _ = process::Command::new(x.trim()).spawn();
    });
}

