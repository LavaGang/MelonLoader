pub fn is_32_bit() -> bool {
    cfg!(target_pointer_width = "32")
}
