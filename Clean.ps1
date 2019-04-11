gci -fi HackerCalc.* | ? { $_.PSIsContainer } | % {
  $bin = (Join-Path $_.FullName 'bin' )
  if (Test-Path $bin) {
    Write-Host "Removing $($_.Name)/bin"
    Remove-Item $bin -Force -Recurse
  }
  $obj = (Join-Path $_.FullName 'obj' )
  if (Test-Path $obj) {
    Write-Host "Removing $($_.Name)/obj"
    Remove-Item $obj -Force -Recurse
  }
}