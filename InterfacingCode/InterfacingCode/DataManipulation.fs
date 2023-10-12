module DataManipulation

open System
open System.Text.RegularExpressions
open Types
open HyperParams




// Shuffles the order of a list, used to make sure the order of the input csv doesnt bias the allocations
let shuffleList (list: 'a list) : 'a list =
    let rnd = Random(SEED)
    list |> List.sortBy (fun _ -> rnd.Next())

// Takes a string and turns it into a list of strings getting the items from a given pattern. Used on strings like ``Streams Eligible`` from the project proposals csv.
let parseStringOfList string pattern =
    let matches = Regex.Matches(string, pattern)

    matches
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Groups.[1].Value)
    |> List.ofSeq

// Turns a students preferences from the csv into a list of the preferences type
let processPrefences (studentPrefRow: CsvStudentPrefData.Row) =
    [ { ProjectName = studentPrefRow.``1st Preference``
        EqualToAbove = false }
      { ProjectName = studentPrefRow.``2nd Preference``
        EqualToAbove = studentPrefRow.``2nd Preference Equal`` }
      { ProjectName = studentPrefRow.``3rd Preference``
        EqualToAbove = studentPrefRow.``3rd Preference Equal`` }
      { ProjectName = studentPrefRow.``4th Preference``
        EqualToAbove = studentPrefRow.``4th Preference Equal`` }
      { ProjectName = studentPrefRow.``5th Preference``
        EqualToAbove = studentPrefRow.``5th Preference Equal`` }
      { ProjectName = studentPrefRow.``6th Preference``
        EqualToAbove = studentPrefRow.``6th Preference Equal`` }
      { ProjectName = studentPrefRow.``7th Preference``
        EqualToAbove = studentPrefRow.``7th Preference Equal`` }
      { ProjectName = studentPrefRow.``8th Preference``
        EqualToAbove = studentPrefRow.``8th Preference Equal`` } ]


// Given a list of supervisors, looks up the superviosr email and if exists in superviosrs then increases the supervisors NumberOfSuperviosrs and NumberOfProjects by a given ammount. If the supervisor email does not exist in the list then it adds a new supervisor to the list with the increase values increasing the default of 0.
let createOrUpdateSupervisor
    (csvSupervisorLoadingData: CsvSupervisorLoadingData)
    supervisors
    supervisorEmail
    increaseNumberOfAllocationsBy
    increaseNumberOfProjectsBy
    =
    let findSupervisorInfo (supervisorEmail: string) =
        let supervisorOption =
            Seq.tryFind
                (fun (row: CsvSupervisorLoadingData.Row) -> row.``Supervisor Email`` = supervisorEmail)
                csvSupervisorLoadingData.Rows

        match supervisorOption with
        | Some result -> result
        | None ->
            failwithf
                "There is a project with supervisor %A who does not have an entry in the supervisor loading list"
                supervisorEmail

    match List.tryFind (fun supervisor -> supervisor.Email = supervisorEmail) supervisors with
    | None ->
        let supervisorInfo = findSupervisorInfo supervisorEmail

        if supervisorInfo.``Max Projects`` < increaseNumberOfAllocationsBy then
            failwithf
                "%A has approved allocations requests and self-proposals that total more than their max loading"
                supervisorEmail

        let newSupervisor: supervisor =
            { Email = supervisorEmail
              MaxLoading = supervisorInfo.``Max Projects``
              NumberOfAllocations = increaseNumberOfAllocationsBy
              TotalCopiesOfProjects = increaseNumberOfProjectsBy
              CostWeight = float supervisorInfo.``Cost Weight`` }

        newSupervisor :: supervisors
    | Some supervisor ->
        if supervisor.MaxLoading < supervisor.NumberOfAllocations + increaseNumberOfAllocationsBy then
            failwithf
                "%A has approved allocations requests and self-proposals that total more than their max loading"
                supervisorEmail

        let updatedSupervisor =
            { supervisor with
                NumberOfAllocations = supervisor.NumberOfAllocations + increaseNumberOfAllocationsBy
                TotalCopiesOfProjects = supervisor.TotalCopiesOfProjects + increaseNumberOfProjectsBy }

        List.map
            (fun sup ->
                if sup.Email = supervisor.Email then
                    updatedSupervisor
                else
                    sup)
            supervisors

// Looks at the relavent CSVs and generates a list of neccessaryProjectData that has all projects from the ProjectProposals CSV processed. It also returns A list of supervisors who supervise the projects and a list of allocations that came from allocation requests
let processProjects (csvProjectData: CsvProjectData) (csvDuplicationRequestsData: CsvDuplicationRequestData) (csvAllocationRequestsData: CsvAllocationRequestData) (csvSupervisorLoadingData: CsvSupervisorLoadingData) =
    let getAllocationsAndNumberOfCopies (row: CsvProjectData.Row) =
        let copiesFromDupRequest =
            match
                Seq.tryFind
                    (fun (requestRow: CsvDuplicationRequestData.Row) ->
                        (requestRow.ProjectItem = row.Title)
                        && not requestRow.Rejected
                        && requestRow.``Coordinator Approval``)
                    csvDuplicationRequestsData.Rows
            with
            | Some result -> result.NumberOfCopies
            | None -> 0

        let (allocations: allocation list), (CopiesFromAllocationWithDup: int) =
            match
                Seq.filter
                    (fun (requestRow: CsvAllocationRequestData.Row) ->
                        requestRow.Title = row.Title
                        && not requestRow.Rejected
                        && requestRow.``Student Aproval``
                        && requestRow.``Coordinator Approval``)
                    csvAllocationRequestsData.Rows
            with
            | seq when Seq.length seq = 0 -> [], 0
            | seqOfAllocations ->
                if row.``Removed By Supervisor`` then
                    failwithf
                        "An approved allocation request exists for %A which has been removed by its supervisor, please remove the request."
                        row.Title
                else
                    seqOfAllocations
                    |> Seq.map (fun (requestRow: CsvAllocationRequestData.Row) ->
                        ({ StudentEmail = requestRow.``Student Email``
                           ProjectTitle = requestRow.ProjectItem
                           Preference = -1 },
                         requestRow.Duplicate))

                    |> Seq.fold
                        (fun (allocations, copies) (allocation, duplicate) ->
                            (allocation :: allocations, copies + System.Convert.ToInt32(duplicate)))
                        ([], 0)

        allocations, CopiesFromAllocationWithDup + copiesFromDupRequest + 1

    let processRow row =
        let allocations, numberOfCopies = getAllocationsAndNumberOfCopies row

        let parseSuitabilityPattern = @"([^;]+)"

        let parsedUnrankedStudents =
            parseStringOfList row.``Students awaiting ranking`` parseSuitabilityPattern

        let parsedLowSuitabilityStudents =
            parseStringOfList row.``Low Suitability Students`` parseSuitabilityPattern

        let parsedMediumSuitabilityStudents =
            parseStringOfList row.``Medium suitability students`` parseSuitabilityPattern


        let parsedHighSuitabilityStudents =
            parseStringOfList row.``Highly suitable students`` parseSuitabilityPattern


        let parsedUnsuitableStudents =
            parseStringOfList row.``Unsuitable students`` parseSuitabilityPattern

        let parseEligibleStreamsPattern = "\"(.*?)\""

        let parsedEligibleStreams =
            parseStringOfList row.``Streams Eligible`` parseEligibleStreamsPattern

        if List.length allocations > numberOfCopies then
            failwithf "%A has more aproved allocation request than copies" row.Title

        { Title = row.Title
          SupervisorEmail = row.``Supervisor Email``
          SupervisorName = row.Supervisor
          EligibleStreams = parsedEligibleStreams
          LowSuitabilityStudents = parsedLowSuitabilityStudents
          MediumSuitabilityStudents = parsedMediumSuitabilityStudents
          HighSuitabilityStudents = parsedHighSuitabilityStudents
          UnsuitableStudents = parsedUnsuitableStudents
          UnrankedStudents = parsedUnrankedStudents
          DisableLevelsOfSuitability = row.``Disable levels of suitability``
          RemovedBySupervisor = row.``Removed By Supervisor``
          NumberOfAllocations = List.length allocations
          NumberOfCopies = numberOfCopies },
        allocations




    let processedProjects, allocatedProjects, supervisors =
        csvProjectData.Rows
        |> Seq.fold
            (fun (processedProjects, allocations, supervisors) row ->
                let processedProject, allocationsForProject = processRow row

                let checkForExistingAllocation =
                    List.map
                        (fun (newAllocation: allocation) ->
                            List.exists
                                (fun (oldAllocation: allocation) ->
                                    oldAllocation.StudentEmail = newAllocation.StudentEmail)
                                allocations)
                        allocationsForProject

                if List.exists (fun bool -> bool = true) checkForExistingAllocation then
                    failwithf
                        "Multiple approved allocation requests exist for student with email %A"
                        allocationsForProject[List.findIndex (fun bool -> bool = true) checkForExistingAllocation]
                            .StudentEmail

                let updatedSupervisors =
                    createOrUpdateSupervisor
                        csvSupervisorLoadingData
                        supervisors
                        processedProject.SupervisorEmail
                        (List.length (
                            List.distinctBy (fun (request: allocation) -> request.StudentEmail) allocationsForProject
                        ))
                        processedProject.NumberOfCopies

                (processedProject :: processedProjects, allocationsForProject @ allocations, updatedSupervisors)

                )
            ([], [], [])

    (shuffleList processedProjects, allocatedProjects, supervisors)

// Updates the given supervisors list to add any new supervisors just doing self proposals and update exsisting superviosrs if they are supervising a self proposal. It also updates the given allocations list to include allocations from self proposals
let processSelfProposals (csvSelfProposalsData: CsvSelfProposalData)
    (csvSupervisorLoadingData: CsvSupervisorLoadingData)
    (supervisors: supervisor list)
    (allocations: allocation list)
    =
    let aprovedSelfProposals =
        Seq.filter
            (fun (proposal: CsvSelfProposalData.Row) ->
                not proposal.Rejected
                && proposal.``Supervisor Approval``
                && proposal.``Coordinator Approval``)
            csvSelfProposalsData.Rows

    let updatedSupervisors, updatedAllocations =
        aprovedSelfProposals
        |> Seq.fold
            (fun (supervisors, allocations) proposal ->
                if
                    List.exists
                        (fun (allocation: allocation) -> allocation.StudentEmail = proposal.``Student Email``)
                        allocations
                then
                    failwithf
                        "multiple allocation requests / self proposals are aproved for student with email %A"
                        proposal.``Student Email``

                let allocation =
                    { StudentEmail = proposal.``Student Email``
                      ProjectTitle = proposal.Title
                      Preference = -2 }

                let updatedSupervisors =
                    createOrUpdateSupervisor csvSupervisorLoadingData supervisors proposal.``Supervisor Email`` 1 1

                (updatedSupervisors, allocation :: allocations))
            (supervisors, allocations)

    (updatedSupervisors, updatedAllocations)

// Processes the StudentsPrefsCSV turning each set of preferences into a neccessaryStudentData removing any perfeces to projects that are not avalible to them and reordering the preferences. It returns the processed list and a lists of names of students that had their preferences altered and students that had preferenecs removed alltogether due to opting into doc allocation or already being allocated a project or having an accepted self proposal.
let processStudentPrefsCSV (csvStudentData: CsvStudentData) (csvStudentPrefData: CsvStudentPrefData) projects allocations supervisors =
    let parseStream (student: CsvStudentData.Row) =
        match student.ERegyr with
        | "E3" -> "EEE BEng"
        | "D4" -> "EEE MEng EM"
        | "J4" -> "EIE MEng"
        | "I3" -> "EIE BEng"
        | "T4" -> "EEE MEng T"
        | "O1" -> "Exchange Students"
        | other -> failwithf "%A has unhandled stream %A" student.Name other

    let setInvalidPrefEmpty stream pref : Preference =
        let projectName = pref.ProjectName

        match projectName with
        | "" -> pref
        | _ ->
            let projectInfo =
                match Seq.tryFind (fun project -> project.Title = projectName) projects with
                | None ->
                    failwithf
                        "A preference for project %A was found this project does not exist in project proposals"
                        pref
                | Some result -> result

            let projectNotRemovedBySupervisor = not projectInfo.RemovedBySupervisor

            let projectNotFullyAllocated =
                projectInfo.NumberOfCopies > List.length (
                    List.filter (fun allocation -> allocation.ProjectTitle = projectInfo.Title) allocations
                )

            let validStream =
                List.exists (fun eligibleStream -> eligibleStream = stream) projectInfo.EligibleStreams


            let supervisorNotAtMaxCapacity =
                match List.tryFind (fun supervisor -> supervisor.Email = projectInfo.SupervisorEmail) supervisors with
                | Some result -> result
                | None ->
                    failwithf
                        "%A exists but its supervisor %A does not exist in supervisor list"
                        projectInfo.Title
                        projectInfo.SupervisorEmail
                |> (fun supervisor -> supervisor.MaxLoading > supervisor.NumberOfAllocations)

            if
                validStream
                && supervisorNotAtMaxCapacity
                && projectNotFullyAllocated
                && projectNotRemovedBySupervisor
            then
                pref
            else
                { pref with ProjectName = "" }

    let setDuplicatePrefsEmpty prefs =
        let folder nonDuplicatePrefs pref =
            if List.exists ((=) pref.ProjectName) (List.map (fun pref -> pref.ProjectName) nonDuplicatePrefs) then
                { ProjectName = ""
                  EqualToAbove = pref.EqualToAbove }
                :: nonDuplicatePrefs
            else
                pref :: nonDuplicatePrefs

        List.rev <| List.fold folder [] prefs

    let removeEmpty (prefs: Preference list) =
        let rec removeInvalidHelper acc prevRemovedEqual =
            function
            | [] -> List.rev acc
            | pref :: rest ->
                if pref.ProjectName = "" then
                    match prevRemovedEqual with
                    | Some false when pref.EqualToAbove -> removeInvalidHelper acc (Some false) rest
                    | _ -> removeInvalidHelper acc (Some pref.EqualToAbove) rest
                else
                    let updatedPref =
                        match prevRemovedEqual with
                        | Some false when pref.EqualToAbove -> { pref with EqualToAbove = false }
                        | _ -> pref

                    removeInvalidHelper (updatedPref :: acc) None rest

        removeInvalidHelper [] None prefs
    
    let removeEmptyStringsTail (lst: Preference list) =
        let rec loop acc remainingList =
            match remainingList with
            | [] -> List.rev acc
            | x :: xs when x.ProjectName = "" && List.forall (fun (p: Preference) -> p.ProjectName = "") xs -> List.rev acc
            | x :: xs -> loop (x :: acc) xs
    
        loop [] lst
    

    let modifyRow (row: CsvStudentPrefData.Row) =
        let originalPrefs = processPrefences row



        let studentInfo =
            match
                Seq.tryFind (fun (studentRow: CsvStudentData.Row) -> studentRow.Email = row.Title) csvStudentData.Rows
            with
            | Some result -> result
            | None -> failwithf "%A has no entry in student information CSV" row.Title


        let correctedPrefs =
            let prefsWithNoneInvalid =
                List.map (setInvalidPrefEmpty <| parseStream studentInfo) originalPrefs
                |> setDuplicatePrefsEmpty

            removeEmpty prefsWithNoneInvalid

        let wasChange = ( removeEmptyStringsTail originalPrefs ) <> correctedPrefs

        let alreadyAllocated =
            List.exists (fun (allocation: allocation) -> allocation.StudentEmail = row.Title) allocations


        let preferenceRanks =
            let rankFolder (processedRanks: int list) pref =
                if processedRanks = [] then
                    [ 1 ]
                elif not pref.EqualToAbove then
                    (List.length processedRanks + 1) :: processedRanks
                else
                    (List.head processedRanks) :: processedRanks

            List.rev <| List.fold rankFolder [] correctedPrefs

        let preferenceAndRankList =
            List.zip (List.map (fun pref -> pref.ProjectName) correctedPrefs) preferenceRanks


        let choppedPreferenceAndRankList =
            if preferenceAndRankList.Length > PREFERENCE_CUTOFF then
                List.take PREFERENCE_CUTOFF preferenceAndRankList
            else
                preferenceAndRankList

        let shouldIgnorePrefs =
            let takingDOC =
                match studentInfo.``Taking DOC Project`` with
                | Some true -> true
                | _ -> false

            if takingDOC && alreadyAllocated then failwithf "%A has identified themselves as wanted to be allocated by doc but has an accepted self proposal/allocation request" row.StudentItem
            
            takingDOC || alreadyAllocated

        { StudentEmail = row.Title
          StudentName = row.StudentItem
          Preferences = choppedPreferenceAndRankList
          Stream = parseStream studentInfo },
        shouldIgnorePrefs,
        wasChange

    let correctedRows, alteredStudents, removedStudents =
        csvStudentPrefData.Rows
        |> Seq.fold
            (fun (correctRows, alteredStudents, removedStudents) row ->
                let modifiedRow, shouldIgnorePrefs, wasChange = modifyRow row

                match (shouldIgnorePrefs, wasChange) with
                | true, _ -> (correctRows, alteredStudents, row.StudentItem :: removedStudents)
                | _, true -> (modifiedRow :: correctRows, row.StudentItem :: alteredStudents, removedStudents)
                | _, false -> (modifiedRow :: correctRows, alteredStudents, removedStudents))
            ([], [], [])

    (shuffleList correctedRows, alteredStudents, removedStudents)

// Detects supervisors that have more project spaces on offer than supervision slots left and creates pseudo student groups for them. The groups have the projects the pseudo students need to have zero cost for and the number of them required.
let generatePseudoStudents supervisors projects =
    List.fold
        (fun pseudoStudentGroups supervisor ->
            let numberOfPseudoStudentsNeeded =
                supervisor.TotalCopiesOfProjects - supervisor.MaxLoading

            let supervisorAtMaxCapacity = supervisor.NumberOfAllocations = supervisor.MaxLoading


            if numberOfPseudoStudentsNeeded <= 0 || supervisorAtMaxCapacity then
                pseudoStudentGroups
            else
                let unallocatedSupervisorProjects =
                    List.filter
                        (fun project ->
                            project.SupervisorEmail = supervisor.Email
                            && project.NumberOfCopies > project.NumberOfAllocations)
                        projects



                let newPseudoStudentGroup =
                    match unallocatedSupervisorProjects with
                    | [] ->
                        failwithf
                            "%A has no unallocated projects but more copies of projects than their max loading"
                            supervisor.Email
                    | _ ->
                        { projectsToSelect = unallocatedSupervisorProjects
                          NumberOfPsuedoStudents = numberOfPseudoStudentsNeeded }

                pseudoStudentGroups @ [ newPseudoStudentGroup ])
        []
        supervisors
