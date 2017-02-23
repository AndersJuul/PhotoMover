# PhotoMover
Utility for moving photos from eg Dropbox to my NAS

Installation -- Danish
--
1. Kopier filer i AJF.PhotoMover.Service til �nsket installationsfolder p� din computer. Hos mig ligger de i C:\Program Files\ajf\photomoverservice\
2. Rediger filen i AJF.PhotoMover.Service.exe.config i installationsfolder:
	Denne indstilling fort�ller hvilken fil, der skal logges til -- alts� hvor programmet skriver kommentarer om hvad der sker. Nyttigt hvis programmet ikke opf�rer sig som forventet.
    <add key ="RollingFile" value="c:\Logs\ajf.photomover.log.txt"/>
	
	Denne indstilling fort�ller hvor programmet skal finde yderligere config-filer. Flere foldere kan angives - i eksemplet her kigger programmet i tre forskellige foldere under min Dropbox-folder samt p� mit SD-kort (hvis indsat).
    <add key="PathList" value="F:\DCIM\;C:\Users\Anders\Dropbox\Camera Uploads\;C:\Users\Anders\Dropbox\Camera Uploads from Victoria\;C:\Users\Anders\Dropbox\Camera Uploads from Isabella\"/>

3. P� *hver* af de placeringer, som er angivet i config-filen ovenfor skal der ligge en fil ved navn photomover.config :
	Der skal v�re en linie som denne for hver filtype som programmet skal h�ndtere
	<SearchPattern value="*.jpg"/>
	
	Destination fort�ller hvor filer flyttes til. 
	{0} erstattes med �r for den flyttede fils datostempel.
	{1} erstattes med m�ned for den flyttede fils datostempel.
	{2} erstattes med dag for den flyttede fils datostempel.
	<Destination value="\\juulnas\qmultimedia\photos\{0}\{0}, {1}\{0}-{1}-{2}\"/>
    
	TestFile er en fil, som der fors�ges oprettet og slettet. Hvis det ikke kan lade sig g�re opgives flytning af filer (pr�ver igen senere)
	<TestFile value ="\\\\juulnas\\qmultimedia\\photos\\a.txt"/>

	MinAgeHours angiver en forsinkelse i timer, idet en fil skal have mindst den angivne alder i timer f�r den flyttes.
	Kan bruges hvis man feks �nsker at fotos forbliver kortvarigt i dropbox-folder s� man kan n� at kopiere, maile, slette etc.
    <MinAgeHours value ="0"/>
4. S� er vi klar til at installere service
	�bn en Kommando-prompt med Administrator-privilegier
	Naviger til mappen med service-filer (trin 1 ovenfor) og udf�r 
	AJF.PhotoMover.Service.exe install

	Du burde nu kunne finde service i Service-vinduet under Kontrolpanel.

	Service kan startes fra Service-vinduet eller fra kommando-prompten ved at skrive
	net start ajf.photomover

	Hvis du senere �nsker at afinstallere skal du i stedet skrive
	>AJF.PhotoMover.Service.exe uninstall

5. Check i logfilen at service k�rer som forventet.