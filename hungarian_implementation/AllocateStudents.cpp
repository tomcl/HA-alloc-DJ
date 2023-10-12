#include <iostream>
#include <filesystem>
#include <fstream>
#include <vector>
#include <sstream>
#include "Hungarian.h"

int main(void)
{

    std::filesystem::path currentPath = std::filesystem::current_path();
    std::string currentDir = currentPath.string();
    std::cout << "Current Working Directory: " << currentDir << std::endl;

	ifstream costsFile;

	costsFile.open(currentDir + "/Matrix_Costs_And_Dimensions.txt");

	if (!costsFile) {
    cerr << "Unable to open file" + currentDir + "/Matrix_Costs_And_Dimensions.txt";
    exit(1);   // call system to stop
}
    
    int outerSize, innerSize;
    costsFile >> outerSize >> innerSize;
    
    // std::cout << outerSize << std::endl;
    // std::cout << innerSize << std::endl;
    std::vector<std::vector<double>> costMatrix(outerSize, std::vector<double>(innerSize));

    std::string line;
    double value;
    int outerIndex = 0;
    int innerIndex = 0;
    
    while (std::getline(costsFile, line)) {
        std::istringstream iss(line);
        while (iss >> value) {
            costMatrix[outerIndex][innerIndex] = value;
            innerIndex++;
            if (innerIndex == innerSize) {
                innerIndex = 0;
                outerIndex++;
                if (outerIndex == outerSize) {
                    break;
                }
            }
        }
        if (outerIndex == outerSize) {
            break;
        }
    }
    
    costsFile.close();

    // Print the data
    // for (const auto& row : costMatrix) {
    //     for (const auto& val : row) {
    //         std::cout << val << " ";
    //     }
    //     std::cout << std::endl;
    // }

	HungarianAlgorithm HungAlgo;
	vector<int> assignment;

	double cost = HungAlgo.Solve(costMatrix, assignment);

    std::ofstream outputFile("../InterfacingCode/InterfacingCode/assignments.txt");

	for (unsigned int x = 0; x < costMatrix.size(); x++){
		outputFile << x << "," << assignment[x] << "\n";
		// std::cout << x << "," << assignment[x] << "\n";
    }
    outputFile << cost << std::endl;
    // std::cout << cost << std::endl;
    
    outputFile.close();

	return 0;
}
