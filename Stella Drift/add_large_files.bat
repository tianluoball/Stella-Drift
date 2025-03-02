@echo off
echo 正在查找大于100MB的文件...
FOR /R %%G IN (*) DO (
  call :check_file_size "%%G"
)
echo 完成！大文件已添加到Git LFS追踪列表
echo 请在GitHub Desktop中提交.gitattributes文件
pause
goto :eof

:check_file_size
set "file=%~1"
FOR %%A IN ("%file%") DO set size=%%~zA
if %size% GTR 104857600 (
  echo 发现大文件: %file% ^(%size% 字节^)
  git lfs track "%file:D:\2025\Metacreation\Stella-Drift\=%"
)
goto :eof