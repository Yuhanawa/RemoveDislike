 use std::env;
 use CleanupModule::*;

fn main() {
    let args: Vec<String> = env::args().collect();
    
    if args.len() < 2 {
        println!("Usage: {} <rule file> [disabled(|_split_|)]", args[0]);
        return;
    }

    let mut disabled = "".to_string();
    
    if args.len() >= 3{
        disabled = args[2].clone();
    }

    
    let len = cleanup(&args[1], disabled);
    println!("{}", size_to_string(len));
}
