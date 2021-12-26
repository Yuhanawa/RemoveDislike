use cleanup_module::*;
use std::env;

fn main() {
    let args: Vec<String> = env::args().collect();

    if args.len() != 2 {
        println!("Usage: {} <rule file>", args[0]);
        return;
    }

    let len = cleanup(&args[1]);
    println!("{}", size_to_string(len));
}
