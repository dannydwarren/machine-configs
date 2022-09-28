#https://www.bestarkhosting.com/guides/how-to-host-an-ark-dedicated-server

cd "C:\ArkDedicatedServer\ShooterGame\Binaries\Win64"
start ShooterGameServer.exe -NoBattlEye "<map-here>?SessionName=<session-name-here>?ServerPassword=<password-secret-here>?ServerAdminPassword=<password-secret-here>?Port=7777?QueryPort=27015?MaxPlayers=10?listen"
exit