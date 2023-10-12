module Types

open FSharp.Data

[<Literal>]
let ResolutionFolder = __SOURCE_DIRECTORY__

type CsvProjectData = CsvProvider<"./csvs/ProjectProposals.csv", ResolutionFolder=ResolutionFolder>
type CsvStudentPrefData = CsvProvider<"./csvs/StudentsPrefs.csv", ResolutionFolder=ResolutionFolder>
type CsvStudentData = CsvProvider<"./csvs/StudentInfo.csv", ResolutionFolder=ResolutionFolder>
type CsvAllocationRequestData = CsvProvider<"./csvs/AllocationRequests.csv", ResolutionFolder=ResolutionFolder>
type CsvDuplicationRequestData = CsvProvider<"./csvs/DuplicationRequests.csv", ResolutionFolder=ResolutionFolder>
type CsvSupervisorLoadingData = CsvProvider<"./csvs/SupervisorLoading.csv", ResolutionFolder=ResolutionFolder>
type CsvSelfProposalData = CsvProvider<"./csvs/SelfProposals.csv", ResolutionFolder=ResolutionFolder>

type allocation =
    { StudentEmail: string
      ProjectTitle: string
      // -1 = allocation request, -2 = self proposal, -3 unallocated
      Preference: int }

type supervisor =
    { Email: string
      MaxLoading: int
      NumberOfAllocations: int
      TotalCopiesOfProjects: int
      CostWeight: float }

type neccessaryProjectData =
    { Title: string
      SupervisorEmail: string
      SupervisorName: string
      EligibleStreams: List<string>
      LowSuitabilityStudents: list<string>
      MediumSuitabilityStudents: list<string>
      HighSuitabilityStudents: list<string>
      UnsuitableStudents: list<string>
      UnrankedStudents: list<string>
      DisableLevelsOfSuitability: bool
      RemovedBySupervisor: bool
      NumberOfAllocations: int
      NumberOfCopies: int }


type neccessaryStudentData =
    { StudentEmail: string
      StudentName: string
      Preferences: list<string * int>
      Stream: string }


type Preference =
    { ProjectName: string
      EqualToAbove: bool }

type psuedoStudentGroup =
    { projectsToSelect: list<neccessaryProjectData>
      NumberOfPsuedoStudents: int }
