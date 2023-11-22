use colored::Color;

pub const MELON_VERSION: &str = "1.0.0";

pub const IS_ALPHA: bool = false;

pub const RED: Color = Color::TrueColor {
    r: (255),
    g: (0),
    b: (0),
};

pub const GREEN: Color = Color::TrueColor {
    r: (0),
    g: (255),
    b: (0),
};

pub const BLUE: Color = Color::TrueColor {
    r: (64),
    g: (64),
    b: (255),
};

#[derive(Debug, Clone, Copy)]
pub struct W<T>(pub T);
