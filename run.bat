dotnet run --project .\InterfacingCode\InterfacingCode\InterfacingCode.fsproj
cd .\hungarian_implementation\
g++ -lstdc++fs -std=c++17 -c AllocateStudents.cpp -o main.o
g++ -lstdc++fs -std=c++17 -c Hungarian.cpp -o hung.o
g++ -lstdc++fs -std=c++17 -o AllocateStudents main.o hung.o
.\AllocateStudents.exe
cd ..
dotnet run --project .\InterfacingCode\InterfacingCode\InterfacingCode.fsproj
del .\InterfacingCode\InterfacingCode\assignments.txt  .\hungarian_implementation\main.o .\hungarian_implementation\hung.o .\hungarian_implementation\AllocateStudents.exe .\hungarian_implementation\Matrix_Costs_And_Dimensions.txt