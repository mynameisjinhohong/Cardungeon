REG ADD "HKEY_CLASSES_ROOT\simple.oauth" /v "uri" /t REG_SZ /d %1 /f
start "" "C:\Unity\Cardungeon\CardDungeon\OnlyStrongRabbitCanSurvive.exe" %1
