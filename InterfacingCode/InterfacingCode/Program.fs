open System.IO
open DataManipulation
open Types
open HyperParams




[<EntryPoint>]
let main argv =
    let csvStudentPrefData = CsvStudentPrefData.GetSample()

    let csvSelfProposalsData = CsvSelfProposalData.GetSample()
    
    let csvStudentData = CsvStudentData.GetSample()

    let csvProjectData: CsvProjectData = CsvProjectData.GetSample()
    
    let csvAllocationRequestsData = CsvAllocationRequestData.GetSample()
    
    let csvDuplicationRequestsData = CsvDuplicationRequestData.GetSample()
    
    let csvSupervisorLoadingData = CsvSupervisorLoadingData.GetSample()

    let processedProjects, allocations, supervisors = processProjects  csvProjectData csvDuplicationRequestsData csvAllocationRequestsData csvSupervisorLoadingData

    let updatedSupervisors, updatedAllocations =
        processSelfProposals csvSelfProposalsData csvSupervisorLoadingData supervisors allocations

    let processedStudents, alteredStudents, removedStudents =
        processStudentPrefsCSV csvStudentData csvStudentPrefData processedProjects updatedAllocations updatedSupervisors

    printfn "The following students are not going to be allocated by preferences due to either opting to do a doc project or having an accepted self proposal or allocation request:\n%A" removedStudents
    printfn "The following students had their preferences altered due to:\ngaps,\nnot being the correct stream,\ntheir preference project being removed,\ntheir preference supervisor being at max capacity,\ntheir preference project being at max capacity:\n%A" alteredStudents

    let desiredProjects =
        processedStudents
        |> List.collect (fun student -> student.Preferences |> List.map fst)
        |> List.distinct

    let supervisorsAtMaxLoad =
        updatedSupervisors
        |> List.filter (fun (sup: supervisor) -> sup.NumberOfAllocations >= sup.MaxLoading) 

    // Removes ProjectProposals that are not avaliable for allocation and duplicates ones with mulitple copies avalible for allocation.
    let projectsToBeAllocated, updatedSupervisors =
        let semiProcessedProjects, adjustedSupervisors =
            processedProjects
            |> List.fold 
                (fun (filteredProjects: neccessaryProjectData list,adjustedSupervisors: supervisor list) project -> 
                    let numberOfAvalibleCopies = project.NumberOfCopies - project.NumberOfAllocations
                    if (not <| List.contains project.Title desiredProjects) 
                    then (filteredProjects, createOrUpdateSupervisor csvSupervisorLoadingData adjustedSupervisors project.SupervisorEmail 0 -numberOfAvalibleCopies)
                    else (project :: filteredProjects, adjustedSupervisors)

                    ) 
                ([], updatedSupervisors)
        let fullyProcessedProjects = 
            semiProcessedProjects  
            |> List.filter (fun project ->
                not (supervisorsAtMaxLoad |> List.exists (fun sup -> sup.Email = project.SupervisorEmail) ))
            |> List.filter (fun project ->
                project.NumberOfCopies > project.NumberOfAllocations
                && not project.RemovedBySupervisor)
            |> List.collect (fun project ->
                let numberOfDups = project.NumberOfCopies - project.NumberOfAllocations
                List.replicate numberOfDups project)
        fullyProcessedProjects, adjustedSupervisors
    


    let psuedoStudentGroups =
        generatePseudoStudents updatedSupervisors projectsToBeAllocated

    let psuedoStudentCosts =
        let groupToCostMapping psuedoGroup =
            List.map
                (fun project ->
                    if List.exists (fun pref -> pref.Title = project.Title) psuedoGroup.projectsToSelect then
                        0.0
                    else
                        UNALLOCATION_COST)
                projectsToBeAllocated
            |> List.replicate psuedoGroup.NumberOfPsuedoStudents
            |> List.concat

        List.collect groupToCostMapping psuedoStudentGroups

    let studentsCosts =
        let mapping (student, project) =
            if
                not
                <| List.exists (fun prefAndRank -> fst prefAndRank = project.Title) student.Preferences
            then
                UNALLOCATION_COST
            elif
                not
                <| List.exists
                    (fun name -> name = student.StudentName)
                    (project.LowSuitabilityStudents
                     @ project.MediumSuitabilityStudents
                       @ project.HighSuitabilityStudents @ project.UnrankedStudents)
            then
                UNALLOCATION_COST
            else
                let suitabilityCost =

                    if project.DisableLevelsOfSuitability then
                        DISABLED_SUITABILITY_LEVELS_COST
                    elif List.exists ((=) student.StudentName) project.LowSuitabilityStudents then
                        getSuitabilityCost "low"
                    elif List.exists ((=) student.StudentName) project.MediumSuitabilityStudents then
                        getSuitabilityCost "med"
                    elif List.exists ((=) student.StudentName) project.HighSuitabilityStudents then
                        getSuitabilityCost "high"
                    else
                        getSuitabilityCost "unranked"

                let perferenceCost =
                    match List.tryFind (fun prefAndRank -> fst prefAndRank = project.Title) student.Preferences with
                    | None -> failwith "tried to find preferences for project not in student preferences"
                    | Some(_, rank) -> getPrefCost rank

                let defaultCost =
                    SUITABILITY_MULTIPLIER * suitabilityCost
                    + PREFERENCE_MULTIPLIER * perferenceCost

                let supervisorWeighting =
                    match List.tryFind (fun sup -> sup.Email = project.SupervisorEmail) updatedSupervisors with
                    | None -> failwith "project supervisor does not exist in updated supervisor list"
                    | Some sup -> sup.CostWeight

                match student.Preferences.Length with
                | x when x < REQUIRED_PREFS ->
                    float defaultCost
                    * supervisorWeighting
                    * LESS_THAN_REQUIRED_PREFS_COST_MULTIPLIER
                | _ -> float defaultCost * supervisorWeighting

        List.allPairs processedStudents projectsToBeAllocated |> List.map mapping


    let matrixCosts = studentsCosts @ psuedoStudentCosts
    let costMatrixM = List.length projectsToBeAllocated
    let costMatrixN = List.length matrixCosts / costMatrixM

    let outputFile =
        File.CreateText("../../hungarian_implementation/Matrix_Costs_And_Dimensions.txt")

    outputFile.WriteLine(costMatrixN)
    outputFile.WriteLine(costMatrixM)
    matrixCosts |> List.iter (fun cost -> outputFile.WriteLine(cost))
    outputFile.Close()

    let finalAllocations, finalSupervisors, totalCost =
        let listOfAssignments, algrithmCost =
            try
                let stringList = System.IO.File.ReadLines("./assignments.txt") |> Seq.toList

                let proccessedList =
                    List.take (stringList.Length - 1) stringList
                    |> List.map (fun string ->
                        match string.Split(',') with
                        | [| before; after |] -> (int before, int after)
                        | _ -> failwith "Invalid string format")

                let algorithmsCost =
                    List.rev stringList |> (List.take 1 >> List.exactlyOne >> float)

                proccessedList, algorithmsCost
            with :? FileNotFoundException as ex ->
                printfn "Matrix_Costs_And_Dimensions.txt generated: %s" ex.Message
                exit 0

        let totalCost =
            let costOfUnallocations =
                UNALLOCATION_COST
                * (float
                   <| List.length (List.filter (fun (_, projectIdx) -> projectIdx = -1) listOfAssignments))

            costOfUnallocations + algrithmCost

        let finalAllocations, finalSupervisors =
            let stringToAllocationFolder
                (existsingAllocations: allocation List, existingSupervisors: supervisor list)
                (studentIdx, projectIdx)
                =
                match (studentIdx, projectIdx) with
                | r, c when
                    studentsCosts.Item(r * projectsToBeAllocated.Length + c) = UNALLOCATION_COST
                    || c = -1
                    ->
                    [ { StudentEmail = processedStudents.Item(studentIdx).StudentEmail
                        ProjectTitle = "unallocated"
                        Preference = -3 } ]
                    @ existsingAllocations,
                    existingSupervisors
                | r, c ->
                    let student = processedStudents.Item(r)
                    let project = projectsToBeAllocated.Item(c)

                    let choice =
                        match List.tryFind (fun prefAndRank -> fst prefAndRank = project.Title) student.Preferences with
                        | None -> failwith "student allocated to project not in his preference"
                        | Some projectAndRank -> snd projectAndRank

                    let updatedSupervisors =
                        match List.tryFind (fun sup -> sup.Email = project.SupervisorEmail) existingSupervisors with
                        | None -> failwith "project allocated with supervisor that does not exist in superviors list"
                        | Some sup -> createOrUpdateSupervisor csvSupervisorLoadingData existingSupervisors sup.Email 1 0

                    [ { StudentEmail = student.StudentEmail
                        ProjectTitle = project.Title
                        Preference = choice } ]
                    @ existsingAllocations,
                    updatedSupervisors

            List.take (processedStudents.Length) listOfAssignments
            |> List.fold stringToAllocationFolder (updatedAllocations, updatedSupervisors)

        finalAllocations, finalSupervisors, totalCost



    printfn "\nSTUDENT REVEIW:\n"

    for allocation in finalAllocations do
        printfn "%A -> %A choice: %A" allocation.StudentEmail allocation.ProjectTitle allocation.Preference

    printfn "\nSUPERVISORS REVEIW:\n"

    for sup in finalSupervisors do
        printfn "%A Max: %A Allocated %A" sup.Email sup.MaxLoading sup.NumberOfAllocations



    let preferenceHistogram =
        finalAllocations
        |> List.groupBy (fun allocation -> allocation.Preference)
        |> List.map (fun (preference, group) -> (preference, List.length group))
        |> List.sortBy (fun (preference, _) -> preference)

    printfn "\nPREFERENCE HISTORGRAM:\n"

    preferenceHistogram
    |> List.iter (fun (preference, count) ->
        match preference with
        | -3 -> printfn "Unallocated: %d students" count
        | -2 -> printfn "Self Proposed: %d students" count
        | -1 -> printfn "Allocation Requests: %d students" count
        | _ -> printfn "Preference %d: %d students" preference count)

    printfn "TOTAL COST: %A" <| totalCost

    0 // Exit code
