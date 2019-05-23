
function runTests(){
    testType="$1"

    if [ "$testType" == "i" ]; then
        dotnet test "src/tests/NTrospection.Tests.CLI/NTrospection.Tests.CLI.csproj" --filter TestCategory=Integration
    else
	dotnet test "src/tests/NTrospection.Tests.CLI/NTrospection.Tests.CLI.csproj" --filter TestCategory!=Integration
    fi
    
}

runTests $1
