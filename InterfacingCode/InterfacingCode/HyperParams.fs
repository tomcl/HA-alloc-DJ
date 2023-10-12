module HyperParams

// Seed for random number generation
let SEED = 42
// Hyperparameters
let UNALLOCATION_COST = 100.0 // Adjusts bias towards allocating as many students as possible and punishing students with extra preferences
let SUITABILITY_MULTIPLIER = 2.0 // Adjusts the impact of suitability on the allocation cost
let PREFERENCE_MULTIPLIER = 3.0 // Modifies the influence of preference on the allocation cost
let DISABLED_SUITABILITY_LEVELS_COST = 1.0 // Sets the suitability cost for assigning students to a project with suitability costs disabled
let PREFERENCE_CUTOFF = 8 // Discards preferences above this number to reduce chances of high preferences for students with extra preferences
let REQUIRED_PREFS = 3 // Sets the minimum number of preferences required from students by the coordinator
let LESS_THAN_REQUIRED_PREFS_COST_MULTIPLIER = 1.5 // Affects the cost of allocating a student with fewer than the required number of preferences

// Gets the cost of assigning a student with a given suitability rank to a project. Costs can be easierly altered by fyp coordinator
let getSuitabilityCost rank =
    match rank with
    | "low" -> 3.0
    | "med" -> 2.0
    | "high" -> 1.0
    // Students should not be punished if the supervisor does not rank them
    | "unranked" -> 1.0
    | _ -> failwithf "unhandled case of suitability"

// Gets the cost of assigning a student with a given perference rank to a project. Costs can be easierly altered by fyp coordinator
let getPrefCost prefRank =
    match prefRank with
    | 1 -> 1.0
    | 2 -> 2.0
    | 3 -> 3.0
    | 4 -> 4.0
    | 5 -> 5.0
    | 6 -> 6.0
    | 7 -> 7.0
    | 8 -> 8.0
    | _ -> failwithf "unhandled preference case"
