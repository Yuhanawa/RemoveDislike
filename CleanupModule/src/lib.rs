use serde_json::Value;
use std::{env, fs, io::Read};

pub fn cleanup(path: &str) -> u64 {
    let mut jsonstr = String::new();
    fs::File::open(&path)
        .unwrap()
        .read_to_string(&mut jsonstr)
        .unwrap();

    for (key, value) in env::vars() {
        if "PATH" == key {
            continue;
        }
        jsonstr = jsonstr.replace(
            &format!("%{}%", key),
            &format!("{}/", value.replace("\\", "/")),
        );
    }

    let mut all_len: u64 = 0;

    let v: Value = serde_json::from_str(&jsonstr[..]).unwrap();
    v["rules"]
        .as_object()
        .unwrap()
        .iter()
        .for_each(|(_, rule)| {
            rule.as_object().unwrap().iter().for_each(|(pattern, v)| {
                v.as_array().unwrap().iter().for_each(|path| {
                    let path = path.as_str().unwrap();
                    all_len += file_processing(path, pattern);
                })
            });
        });

    all_len
}

fn file_processing(path: &str, pattern: &String) -> u64 {
    let mut len: u64 = 0;
    let mut tmp: u64 = 0;

    if path.ends_with('/') || path.ends_with('\\') {
        if let Ok(result_list) = globwalk::glob(path.to_owned() + pattern) {
            for f in result_list {
                if let Ok(f) = f {
                    if let Ok(meta) = fs::metadata(f.path()) {
                        tmp = meta.len();
                    }
                    if let Ok(_) = fs::remove_file(f.path()) {
                        println!("{}", f.path().display());
                        len += tmp;
                    } else {
                        println!("!!!ERR: {}", f.path().display());
                    }
                }
            }
        }
    } else {
        if let Ok(meta) = fs::metadata(path) {
            tmp = meta.len();
        }

        if let Ok(_) = fs::remove_file(path) {
            println!("{}", path);
            len += tmp;
        } else {
            println!("!!!ERR: {}", path);
        }

        println!("{}", path);
    }
    len
}

pub fn size_to_string(size: u64) -> String {
    let mut size = size;
    let mut unit = "B";
    if size > 1024 {
        size = size / 1024;
        unit = "KB";
    }
    if size > 1024 {
        size = size / 1024;
        unit = "MB";
    }
    if size > 1024 {
        size = size / 1024;
        unit = "GB";
    }
    if size > 1024 {
        size = size / 1024;
        unit = "TB";
    }
    if size > 1024 {
        size = size / 1024;
        unit = "PB";
    }
    if size > 1024 {
        size = size / 1024;
        unit = "EB";
    }
    if size > 1024 {
        size = size / 1024;
        unit = "ZB";
    }
    if size > 1024 {
        size = size / 1024;
        unit = "YB";
    }
    format!("{} {}", size, unit)
}


#[no_mangle]
pub extern fn Cleanup(path: String) -> u64 {
    cleanup(path.as_str())
}

#[no_mangle]
pub extern fn SizeToStr(size: u64) -> String {
    size_to_string(size)
}