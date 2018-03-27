@echo off

rd E:\WorkSpace\GitHub\Discovery\Assets\ResourceAssets\Config /s /q
md E:\WorkSpace\GitHub\Discovery\Assets\ResourceAssets\Config

rd E:\WorkSpace\GitHub\Discovery\Assets\Scripts\Config /s /q
md E:\WorkSpace\GitHub\Discovery\Assets\Scripts\Config

xcopy Out\JsonData E:\WorkSpace\GitHub\Discovery\Assets\ResourceAssets\Config /s
xcopy Out\JsonCode E:\WorkSpace\GitHub\Discovery\Assets\Scripts\Config /s

pause