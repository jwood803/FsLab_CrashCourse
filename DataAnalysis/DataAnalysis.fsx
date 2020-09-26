#load "packages/FsLab/FsLab.fsx"

open Deedle

// Load data
let bank = Frame.ReadCsv("./bank.csv", hasHeaders=true, separators=";")

// Get columns
let age = bank.GetColumn<int>("age")
age

let missing = bank.TryGetColumn<string>("col", Lookup.Exact)
missing

let items = [| 1; 2 |]

items.[0]

 // Similar to bank["age"] in pandas
let age2 = bank.["age"]
age2

age2.Get(1)

age2.TryGet(100000000)

age2.Observations

age2 |> Series.observationsAll

bank?age

// Stats
Stats.mean bank

Stats.mean bank?age

let describe (frame: Frame<'R, 'C>) = 
  series [
    "Mean" => Stats.mean frame
    "Median" => Stats.median frame
    "Min" => Stats.min frame
    "Max" => Stats.max frame
    "Std" => Stats.stdDev frame
  ]

describe bank

// Math Numerics

#load "packages/MathNet.Numerics.FSharp/MathNet.Numerics.fsx"

open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra

SpecialFunctions.Factorial 5

let m = matrix [
  [1.0; 2.0]
  [3.0; 4.0]
]

let inverse = m |> Matrix.inverse
inverse

m * inverse

// DataFrame Operations

// Filtering
bank.FilterRowsBy("age", 30)

bank |> Frame.filterRowsBy "age" 30 |> Frame.take 5

// Grouping
bank.GroupRowsBy<string>("marital")

bank |> Frame.groupRowsUsing (fun k row ->
  row.GetAs<bool>("default"))

bank |> Frame.groupRowsByString "job" |> Frame.groupRowsByBool "loan"

// Missing values
bank |> Frame.fillMissingWith 0

bank?age |> Series.fillMissing Direction.Backward

bank?age |> Series.dropMissing

bank?age |> Series.fillMissingUsing (fun v ->
  let previous = bank?age.TryGet(v, Lookup.ExactOrSmaller)
  let next = bank?age.TryGet(v, Lookup.ExactOrGreater)

  match previous, next with
  | OptionalValue.Present(p), OptionalValue.Present(n) -> p + n
  | OptionalValue.Present(v), _ 
  | _, OptionalValue.Present(v) -> v
  | _ -> 0.0
)

// Charting

open XPlot.Plotly

Chart.Bar(bank?age)

let layout = Layout(title="Age", showlegend=false)

Chart.Bar(bank?age) |> Chart.WithLayout layout

Chart.Scatter(bank?age |> Series.toVector)