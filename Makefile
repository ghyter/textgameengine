zip: 
	tar -czf textgameengine.tar.gz --exclude='.git' --exclude='bin' --exclude='obj' --exclude='*.tar.gz' .

clean:
	dotnet clean
	rm -rf GameEditor/GameEditor.Server/bin/
	rm -rf GameEditor/GameEditor.Client/bin/
	rm -rf GameEditor/GameEditor.Server/obj/
	rm -rf GameEditor/GameEditor.Client/obj/
	
buildEditor:
	dotnet build GameEditor/GameEditor.Server
