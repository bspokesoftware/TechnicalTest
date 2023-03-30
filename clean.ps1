Get-ChildItem -Path "bin","obj" -Directory -Recurse | ForEach-Object {
    if(Test-Path $_.FullName) {
        Remove-Item $_.FullName -Recurse -Force
    }
}