pub type DynErr = Box<dyn std::error::Error>;

pub mod conerr;
pub mod hookerr;
pub mod logerr;
pub mod dotneterr;
pub mod melonerr;