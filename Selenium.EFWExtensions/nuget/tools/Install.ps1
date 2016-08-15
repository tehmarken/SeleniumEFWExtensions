# this is to make sure chromedriver.exe always gets copied to output directory
param($installPath, $toolsPath, $package, $project)
$file1 = $project.ProjectItems.Item("chromedriver.exe")
 
# set 'Copy To Output Directory' to 'Copy always'
$copyToOutput1 = $file1.Properties.Item("CopyToOutputDirectory")
$copyToOutput1.Value = 1