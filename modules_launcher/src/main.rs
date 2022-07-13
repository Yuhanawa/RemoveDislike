#![windows_subsystem = "windows"]

use std::{env, fs, process};


fn main() {
    fs::read_to_string(env::var("APPDATA").unwrap() + "/RemoveDislike/Modules/Launcher.config").unwrap().split('\n')
        .filter(|x| !x.starts_with("//") && !x.starts_with('#')).for_each(|x| {
        let mut x = x.trim().to_string();

        if let Some(p) = regex::Regex::new("%.+%").unwrap().captures(&x.to_owned()) {
            p.iter().filter(|p| p.is_some()).for_each(|m| {
                x = x.replace(m.unwrap().as_str(), &env::var(m.unwrap().as_str().trim_matches('%')).unwrap());
            })
        }

        let _ = process::Command::new(&x).spawn();
    })
}

