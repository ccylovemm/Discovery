@echo off

rd E:\WorkSpace\RoguelikeGame\Roguelike_Main\Assets\ResourceAssets\Config /s /q
md E:\WorkSpace\RoguelikeGame\Roguelike_Main\Assets\ResourceAssets\Config

rd E:\WorkSpace\RoguelikeGame\Roguelike_Main\Assets\Scripts\Config /s /q
md E:\WorkSpace\RoguelikeGame\Roguelike_Main\Assets\Scripts\Config

xcopy Out\JsonData E:\WorkSpace\RoguelikeGame\Roguelike_Main\Assets\ResourceAssets\Config /s
xcopy Out\JsonCode E:\WorkSpace\RoguelikeGame\Roguelike_Main\Assets\Scripts\Config /s

pause