module tests

open Types
open DataManipulation
open System.IO

open NUnit.Framework

[<Literal>]
let TestsResolutionFolder = __SOURCE_DIRECTORY__

[<TestFixture>]
type ProcessProjectsTests() =
    [<Test>]
    member this.``2 allocations for same project and no unapproved allocation or duplication requests make it through``
        ()
        =
        // Arrange
        let projectDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs1/ProjectProposals.csv")

        let duplicationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs1/DuplicationRequests.csv")

        let allocationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs1/AllocationRequests.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs1/SupervisorLoading.csv")

        let csvProjectData = CsvProjectData.Load(projectDataPath)

        let csvDuplicationRequestsData =
            CsvDuplicationRequestData.Load(duplicationRequestsDataPath)

        let csvAllocationRequestsData =
            CsvAllocationRequestData.Load(allocationRequestsDataPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)

        // Act
        let ((processedProjects: neccessaryProjectData list),
             (allocatedProjects: allocation list),
             (supervisors: supervisor list)) =
            processProjects csvProjectData csvDuplicationRequestsData csvAllocationRequestsData csvSupervisorLoadingData

        let expectedProcessedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 } ]

        let expectedAllocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let expectedSupervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 } ]
        // Assert
        Assert.AreEqual(expectedProcessedProjects, processedProjects)
        Assert.AreEqual(expectedAllocatedProjects, allocatedProjects)
        Assert.AreEqual(expectedSupervisors, supervisors)

    [<Test>]
    member this.``2 allocations for same project not enough copies``() =
        // Arrange
        let projectDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs2/ProjectProposals.csv")

        let duplicationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs2/DuplicationRequests.csv")

        let allocationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs2/AllocationRequests.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs2/SupervisorLoading.csv")

        let csvProjectData = CsvProjectData.Load(projectDataPath)

        let csvDuplicationRequestsData =
            CsvDuplicationRequestData.Load(duplicationRequestsDataPath)

        let csvAllocationRequestsData =
            CsvAllocationRequestData.Load(allocationRequestsDataPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)

        Assert.Throws(fun () ->
            (processProjects
                csvProjectData
                csvDuplicationRequestsData
                csvAllocationRequestsData
                csvSupervisorLoadingData)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``2 allocations for same project not enough loading capacity``() =
        // Arrange
        let projectDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs3/ProjectProposals.csv")

        let duplicationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs3/DuplicationRequests.csv")

        let allocationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs3/AllocationRequests.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs3/SupervisorLoading.csv")

        let csvProjectData = CsvProjectData.Load(projectDataPath)

        let csvDuplicationRequestsData =
            CsvDuplicationRequestData.Load(duplicationRequestsDataPath)

        let csvAllocationRequestsData =
            CsvAllocationRequestData.Load(allocationRequestsDataPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)

        Assert.Throws(fun () ->
            (processProjects
                csvProjectData
                csvDuplicationRequestsData
                csvAllocationRequestsData
                csvSupervisorLoadingData)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``approved allocation request for retracted project``() =
        // Arrange
        let projectDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs4/ProjectProposals.csv")

        let duplicationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs4/DuplicationRequests.csv")

        let allocationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs4/AllocationRequests.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs4/SupervisorLoading.csv")

        let csvProjectData = CsvProjectData.Load(projectDataPath)

        let csvDuplicationRequestsData =
            CsvDuplicationRequestData.Load(duplicationRequestsDataPath)

        let csvAllocationRequestsData =
            CsvAllocationRequestData.Load(allocationRequestsDataPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)

        Assert.Throws(fun () ->
            (processProjects
                csvProjectData
                csvDuplicationRequestsData
                csvAllocationRequestsData
                csvSupervisorLoadingData)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``supervisor doesnt exist in loading list``() =
        // Arrange
        let projectDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs5/ProjectProposals.csv")

        let duplicationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs5/DuplicationRequests.csv")

        let allocationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs5/AllocationRequests.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs5/SupervisorLoading.csv")

        let csvProjectData = CsvProjectData.Load(projectDataPath)

        let csvDuplicationRequestsData =
            CsvDuplicationRequestData.Load(duplicationRequestsDataPath)

        let csvAllocationRequestsData =
            CsvAllocationRequestData.Load(allocationRequestsDataPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)

        Assert.Throws(fun () ->
            (processProjects
                csvProjectData
                csvDuplicationRequestsData
                csvAllocationRequestsData
                csvSupervisorLoadingData)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``Multiple approved allocation requests for same person``() =
        // Arrange
        let projectDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs6/ProjectProposals.csv")

        let duplicationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs6/DuplicationRequests.csv")

        let allocationRequestsDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs6/AllocationRequests.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs6/SupervisorLoading.csv")

        let csvProjectData = CsvProjectData.Load(projectDataPath)

        let csvDuplicationRequestsData =
            CsvDuplicationRequestData.Load(duplicationRequestsDataPath)

        let csvAllocationRequestsData =
            CsvAllocationRequestData.Load(allocationRequestsDataPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)

        Assert.Throws(fun () ->
            (processProjects
                csvProjectData
                csvDuplicationRequestsData
                csvAllocationRequestsData
                csvSupervisorLoadingData)
            |> ignore)
        |> ignore


type ProcessSelfProposalsTests() =
    [<Test>]
    member this.``Approved self proposal to existing supervisor and non approved not accepted``() =
        // Arrange
        let selfProposalsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs7/SelfProposals.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs7/SupervisorLoading.csv")

        let selfProposalsData = CsvSelfProposalData.Load(selfProposalsPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 } ]

        let updatedSupervisors, updatedAllocations =
            processSelfProposals selfProposalsData csvSupervisorLoadingData supervisors allocatedProjects

        let expectedAllocatedProjects: allocation list =
            [ { StudentEmail = "xuan.cai20@imperial.ac.uk"
                ProjectTitle = "Fun project"
                Preference = -2 }
              { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let expectedSupervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 3
                TotalCopiesOfProjects = 7
                CostWeight = 1.0 } ]
        // Assert
        Assert.AreEqual(expectedAllocatedProjects, updatedAllocations)
        Assert.AreEqual(expectedSupervisors, updatedSupervisors)



    [<Test>]
    member this.``Approved self proposal to existing supervisor with full loading``() =
        // Arrange
        let selfProposalsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs8/SelfProposals.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs8/SupervisorLoading.csv")

        let selfProposalsData = CsvSelfProposalData.Load(selfProposalsPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 2
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 } ]

        Assert.Throws(fun () ->
            (processSelfProposals selfProposalsData csvSupervisorLoadingData supervisors allocatedProjects)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``Approved self proposal to new supervisor with full loading``() =
        // Arrange
        let selfProposalsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs9/SelfProposals.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs9/SupervisorLoading.csv")

        let selfProposalsData = CsvSelfProposalData.Load(selfProposalsPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let supervisors: supervisor list = []

        Assert.Throws(fun () ->
            (processSelfProposals selfProposalsData csvSupervisorLoadingData supervisors allocatedProjects)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``Approved self proposal to new supervisor``() =
        // Arrange
        let selfProposalsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs10/SelfProposals.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs10/SupervisorLoading.csv")

        let selfProposalsData = CsvSelfProposalData.Load(selfProposalsPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 } ]

        let updatedSupervisors, updatedAllocations =
            processSelfProposals selfProposalsData csvSupervisorLoadingData supervisors allocatedProjects

        let expectedAllocatedProjects: allocation list =
            [ { StudentEmail = "xuan.cai20@imperial.ac.uk"
                ProjectTitle = "Fun project"
                Preference = -2 }
              { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let expectedSupervisors: supervisor list =
            [ { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 }
              { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 } ]

        // Assert
        Assert.AreEqual(expectedAllocatedProjects, updatedAllocations)
        Assert.AreEqual(expectedSupervisors, updatedSupervisors)


    [<Test>]
    member this.``Approved self proposal by student who is already allocated``() =
        // Arrange
        let selfProposalsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs11/SelfProposals.csv")

        let supervisorLoadingDataPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs11/SupervisorLoading.csv")

        let selfProposalsData = CsvSelfProposalData.Load(selfProposalsPath)

        let csvSupervisorLoadingData =
            CsvSupervisorLoadingData.Load(supervisorLoadingDataPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let supervisors: supervisor list = []

        Assert.Throws(fun () ->
            (processSelfProposals selfProposalsData csvSupervisorLoadingData supervisors allocatedProjects)
            |> ignore)
        |> ignore

type ProcessStudentPrefTests() =
    [<Test>]
    member this.``Already allocated removed and correct prefs not being altered``() =
        // Arrange
        let StudentInfoPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs12/StudentInfo.csv")

        let StudentPrefsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs12/StudentsPrefs.csv")

        let StudentInfoData = CsvStudentData.Load(StudentInfoPath)

        let StudentPrefsData = CsvStudentPrefData.Load(StudentPrefsPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 }
              { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test1"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test5"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 } ]

        let students, alteredStudents, removedStudents =
            processStudentPrefsCSV StudentInfoData StudentPrefsData processedProjects allocatedProjects supervisors

        let expectedStudents: neccessaryStudentData list =
            [ { StudentEmail = "james.nock19@imperial.ac.uk"
                StudentName = "James"
                Preferences = [ ("test1", 1); ("test5", 1); ("test2", 3) ]
                Stream = "EIE MEng" }
              { StudentEmail = "louise.davis20@imperial.ac.uk"
                StudentName = "Louise"
                Preferences = [ ("test3", 1); ("test4", 1); ("test1", 3); ("test2", 3) ]
                Stream = "EEE MEng EM" } ]

        let expectedAlteredStudents: string list = []

        let expectedRemovedStudents: string list = [ "Sherif" ]
        // Assert
        Assert.AreEqual(expectedStudents, students)
        Assert.AreEqual(expectedAlteredStudents, alteredStudents)
        Assert.AreEqual(expectedRemovedStudents, removedStudents)

    [<Test>]
    member this.``Invaild prefs handled correctly``() =
        // Arrange
        let StudentInfoPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs13/StudentInfo.csv")

        let StudentPrefsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs13/StudentsPrefs.csv")

        let StudentInfoData = CsvStudentData.Load(StudentInfoPath)

        let StudentPrefsData = CsvStudentPrefData.Load(StudentPrefsPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test4"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 }
              { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams = [ "EEE BEng"; "EIE BEng"; "EIE MEng"; "EEE MEng T"; "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "jeff@imperial.ac.uk"
                SupervisorName = "jeff"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test5"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = true
                NumberOfAllocations = 2
                NumberOfCopies = 5 } ]

        let students, alteredStudents, removedStudents =
            processStudentPrefsCSV StudentInfoData StudentPrefsData processedProjects allocatedProjects supervisors


        let expectedStudents: neccessaryStudentData list =
            [ { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                StudentName = "Sherif"
                Preferences = [ ("test1", 1); ("test2", 1) ]
                Stream = "EIE BEng" }
              { StudentEmail = "james.nock19@imperial.ac.uk"
                StudentName = "James"
                Preferences = [ ("test1", 1); ("test2", 2) ]
                Stream = "EIE MEng" }
              { StudentEmail = "louise.davis20@imperial.ac.uk"
                StudentName = "Louise"
                Preferences = [ ("test2", 1) ]
                Stream = "EEE MEng EM" } ]

        let expectedAlteredStudents: string list = [ "Louise"; "James"; "Sherif" ]

        let expectedRemovedStudents: string list = []
        // Assert
        Assert.AreEqual(expectedStudents, students)
        Assert.AreEqual(expectedAlteredStudents, alteredStudents)
        Assert.AreEqual(expectedRemovedStudents, removedStudents)

    [<Test>]
    member this.``DoC prefs removed``() =
        // Arrange
        let StudentInfoPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs14/StudentInfo.csv")

        let StudentPrefsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs14/StudentsPrefs.csv")

        let StudentInfoData = CsvStudentData.Load(StudentInfoPath)

        let StudentPrefsData = CsvStudentPrefData.Load(StudentPrefsPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test4"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 }
              { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams = [ "EEE BEng"; "EIE BEng"; "EIE MEng"; "EEE MEng T"; "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "jeff@imperial.ac.uk"
                SupervisorName = "jeff"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test5"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = true
                NumberOfAllocations = 2
                NumberOfCopies = 5 } ]

        let students, alteredStudents, removedStudents =
            processStudentPrefsCSV StudentInfoData StudentPrefsData processedProjects allocatedProjects supervisors

        let expectedStudents: neccessaryStudentData list =
            [ { StudentEmail = "james.nock19@imperial.ac.uk"
                StudentName = "James"
                Preferences = [ ("test1", 1); ("test2", 2) ]
                Stream = "EIE MEng" }
              { StudentEmail = "louise.davis20@imperial.ac.uk"
                StudentName = "Louise"
                Preferences = [ ("test2", 1) ]
                Stream = "EEE MEng EM" } ]

        let expectedAlteredStudents: string list = [ "Louise"; "James" ]

        let expectedRemovedStudents: string list = [ "Sherif" ]
        // Assert
        Assert.AreEqual(expectedStudents, students)
        Assert.AreEqual(expectedAlteredStudents, alteredStudents)
        Assert.AreEqual(expectedRemovedStudents, removedStudents)

    [<Test>]
    member this.``student doesnt exist in info list``() =
        // Arrange
        let StudentInfoPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs15/StudentInfo.csv")

        let StudentPrefsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs15/StudentsPrefs.csv")

        let StudentInfoData = CsvStudentData.Load(StudentInfoPath)

        let StudentPrefsData = CsvStudentPrefData.Load(StudentPrefsPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test4"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 }
              { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams = [ "EEE BEng"; "EIE BEng"; "EIE MEng"; "EEE MEng T"; "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "jeff@imperial.ac.uk"
                SupervisorName = "jeff"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test5"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = true
                NumberOfAllocations = 2
                NumberOfCopies = 5 } ]

        Assert.Throws(fun () ->
            (processStudentPrefsCSV StudentInfoData StudentPrefsData processedProjects allocatedProjects supervisors)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``project doesnt exist in project list``() =
        // Arrange
        let StudentInfoPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs16/StudentInfo.csv")

        let StudentPrefsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs16/StudentsPrefs.csv")

        let StudentInfoData = CsvStudentData.Load(StudentInfoPath)

        let StudentPrefsData = CsvStudentPrefData.Load(StudentPrefsPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "m.halimi@imperial.ac.uk"
                ProjectTitle = "test4"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 }
              { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams = [ "EEE BEng"; "EIE BEng"; "EIE MEng"; "EEE MEng T"; "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "jeff@imperial.ac.uk"
                SupervisorName = "jeff"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test5"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = true
                NumberOfAllocations = 2
                NumberOfCopies = 5 } ]

        Assert.Throws(fun () ->
            (processStudentPrefsCSV StudentInfoData StudentPrefsData processedProjects allocatedProjects supervisors)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``Identified as taking DoC but already allocated``() =
        // Arrange
        let StudentInfoPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs17/StudentInfo.csv")

        let StudentPrefsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs17/StudentsPrefs.csv")

        let StudentInfoData = CsvStudentData.Load(StudentInfoPath)

        let StudentPrefsData = CsvStudentPrefData.Load(StudentPrefsPath)


        let allocatedProjects: allocation list =
            [ { StudentEmail = "sherif.agbabiaka20@imperial.ac.uk"
                ProjectTitle = "test4"
                Preference = -1 } ]

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 }
              { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams = [ "EEE BEng"; "EIE BEng"; "EIE MEng"; "EEE MEng T"; "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "jeff@imperial.ac.uk"
                SupervisorName = "jeff"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test5"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = true
                NumberOfAllocations = 2
                NumberOfCopies = 5 } ]

        Assert.Throws(fun () ->
            (processStudentPrefsCSV StudentInfoData StudentPrefsData processedProjects allocatedProjects supervisors)
            |> ignore)
        |> ignore

    [<Test>]
    member this.``Pref for project with supervisor not in list``() =
        // Arrange
        let StudentInfoPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs17/StudentInfo.csv")

        let StudentPrefsPath =
            Path.Combine(TestsResolutionFolder, "csvs/testCSVs17/StudentsPrefs.csv")

        let StudentInfoData = CsvStudentData.Load(StudentInfoPath)

        let StudentPrefsData = CsvStudentPrefData.Load(StudentPrefsPath)


        let allocatedProjects: allocation list = []

        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 20000
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 6
                CostWeight = 1.0 }
              { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams = [ "EEE BEng"; "EIE BEng"; "EIE MEng"; "EEE MEng T"; "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "jeff@imperial.ac.uk"
                SupervisorName = "jeff"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test5"
                SupervisorEmail = "dominic.justie-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = true
                NumberOfAllocations = 2
                NumberOfCopies = 5 } ]

        Assert.Throws(fun () ->
            (processStudentPrefsCSV StudentInfoData StudentPrefsData processedProjects allocatedProjects supervisors)
            |> ignore)
        |> ignore

type generatePseudoStudentsTests() =
    [<Test>]
    member this.``4 students needed``() =
        // Arrange
        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 6
                NumberOfAllocations = 2
                TotalCopiesOfProjects = 10
                CostWeight = 1.0 }
              { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 2
                NumberOfCopies = 5 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 2 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 }
              { Title = "test5"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 0
                NumberOfCopies = 1 } ]

        let pseudoStudentGroups: psuedoStudentGroup list =
            generatePseudoStudents supervisors processedProjects

        let expectedPseudoStudentGroups: psuedoStudentGroup list =
            [ { projectsToSelect =
                  [ { Title = "test1"
                      SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                      SupervisorName = "Justice Konec, Dominic"
                      EligibleStreams =
                        [ "EEE BEng"
                          "EEE MEng EM"
                          "EIE BEng"
                          "EIE MEng"
                          "EEE MEng T"
                          "Exchange Students" ]
                      LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                      MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                      HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                      UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                      UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                      DisableLevelsOfSuitability = false
                      RemovedBySupervisor = false
                      NumberOfAllocations = 2
                      NumberOfCopies = 5 }
                    { Title = "test2"
                      SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                      SupervisorName = "Justice Konec, Dominic"
                      EligibleStreams =
                        [ "EEE BEng"
                          "EEE MEng EM"
                          "EIE BEng"
                          "EIE MEng"
                          "EEE MEng T"
                          "Exchange Students" ]
                      LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                      MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                      HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                      UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                      UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                      DisableLevelsOfSuitability = false
                      RemovedBySupervisor = false
                      NumberOfAllocations = 0
                      NumberOfCopies = 1 }
                    { Title = "test3"
                      SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                      SupervisorName = "Justice Konec, Dominic"
                      EligibleStreams =
                        [ "EEE BEng"
                          "EEE MEng EM"
                          "EIE BEng"
                          "EIE MEng"
                          "EEE MEng T"
                          "Exchange Students" ]
                      LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                      MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                      HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                      UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                      UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                      DisableLevelsOfSuitability = false
                      RemovedBySupervisor = false
                      NumberOfAllocations = 0
                      NumberOfCopies = 2 }
                    { Title = "test4"
                      SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                      SupervisorName = "Justice Konec, Dominic"
                      EligibleStreams =
                        [ "EEE BEng"
                          "EEE MEng EM"
                          "EIE BEng"
                          "EIE MEng"
                          "EEE MEng T"
                          "Exchange Students" ]
                      LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                      MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                      HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                      UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                      UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                      DisableLevelsOfSuitability = false
                      RemovedBySupervisor = false
                      NumberOfAllocations = 0
                      NumberOfCopies = 1 }
                    { Title = "test5"
                      SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                      SupervisorName = "Justice Konec, Dominic"
                      EligibleStreams =
                        [ "EEE BEng"
                          "EEE MEng EM"
                          "EIE BEng"
                          "EIE MEng"
                          "EEE MEng T"
                          "Exchange Students" ]
                      LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                      MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                      HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                      UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                      UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                      DisableLevelsOfSuitability = false
                      RemovedBySupervisor = false
                      NumberOfAllocations = 0
                      NumberOfCopies = 1 } ]
                NumberOfPsuedoStudents = 4 }

              ]

        Assert.AreEqual(expectedPseudoStudentGroups, pseudoStudentGroups)

    [<Test>]
    member this.``No unallocated projects but more copies than max loading and not at max capacity``() =
        // Arrange
        let supervisors: supervisor list =
            [ { Email = "dominic.justice-konec20@imperial.ac.uk"
                MaxLoading = 6
                NumberOfAllocations = 5
                TotalCopiesOfProjects = 7
                CostWeight = 1.0 }
              { Email = "jeff@imperial.ac.uk"
                MaxLoading = 1
                NumberOfAllocations = 1
                TotalCopiesOfProjects = 1
                CostWeight = 1.0 } ]

        let processedProjects: neccessaryProjectData list =
            [ { Title = "test1"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test2"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test3"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test4"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 }
              { Title = "test5"
                SupervisorEmail = "dominic.justice-konec20@imperial.ac.uk"
                SupervisorName = "Justice Konec, Dominic"
                EligibleStreams =
                  [ "EEE BEng"
                    "EEE MEng EM"
                    "EIE BEng"
                    "EIE MEng"
                    "EEE MEng T"
                    "Exchange Students" ]
                LowSuitabilityStudents = [ "Lau, Cheuk Hang"; "Staal, Simon T A" ]
                MediumSuitabilityStudents = [ "Sand, Joachim"; "Vandenberghe, Scott" ]
                HighSuitabilityStudents = [ "Nock, James P"; "Davis, Louise" ]
                UnsuitableStudents = [ "Cai, Xuan"; "Popat, Kia" ]
                UnrankedStudents = [ "Agbabiaka, Sherif"; "Necchi, Lucia" ]
                DisableLevelsOfSuitability = false
                RemovedBySupervisor = false
                NumberOfAllocations = 1
                NumberOfCopies = 1 } ]

        Assert.Throws(fun () ->
            (generatePseudoStudents supervisors processedProjects)
            |> ignore)
        |> ignore