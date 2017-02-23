# PhotoMover
Utility for moving photos from eg Dropbox to my NAS

Installation -- Danish
--
1. Kopier filer i AJF.PhotoMover.Service til ønsket installationsfolder på din computer. Hos mig ligger de i C:\Program Files\ajf\photomoverservice\
2. Rediger filen i AJF.PhotoMover.Service.exe.config i installationsfolder:
	Denne indstilling fortæller hvilken fil, der skal logges til -- altså hvor programmet skriver kommentarer om hvad der sker. Nyttigt hvis programmet ikke opfører sig som forventet.
    <add key ="RollingFile" value="c:\Logs\ajf.photomover.log.txt"/>
	
	Denne indstilling fortæller hvor programmet skal finde yderligere config-filer. Flere foldere kan angives - i eksemplet her kigger programmet i tre forskellige foldere under min Dropbox-folder samt på mit SD-kort (hvis indsat).
    <add key="PathList" value="F:\DCIM\;C:\Users\Anders\Dropbox\Camera Uploads\;C:\Users\Anders\Dropbox\Camera Uploads from Victoria\;C:\Users\Anders\Dropbox\Camera Uploads from Isabella\"/>

3. På *hver* af de placeringer, som er angivet i config-filen ovenfor skal der ligge en fil ved navn photomover.config :
	Der skal være en linie som denne for hver filtype som programmet skal håndtere
	<SearchPattern value="*.jpg"/>
	
	Destination fortæller hvor filer flyttes til. 
	{0} erstattes med år for den flyttede fils datostempel.
	{1} erstattes med måned for den flyttede fils datostempel.
	{2} erstattes med dag for den flyttede fils datostempel.
	<Destination value="\\juulnas\qmultimedia\photos\{0}\{0}, {1}\{0}-{1}-{2}\"/>
    
	TestFile er en fil, som der forsøges oprettet og slettet. Hvis det ikke kan lade sig gøre opgives flytning af filer (prøver igen senere)
	<TestFile value ="\\\\juulnas\\qmultimedia\\photos\\a.txt"/>

	MinAgeHours angiver en forsinkelse i timer, idet en fil skal have mindst den angivne alder i timer før den flyttes.
	Kan bruges hvis man feks ønsker at fotos forbliver kortvarigt i dropbox-folder så man kan nå at kopiere, maile, slette etc.
    <MinAgeHours value ="0"/>
4. Så er vi klar til at installere service
	Åbn en Kommando-prompt med Administrator-privilegier
	Naviger til mappen med service-filer (trin 1 ovenfor) og udfør 
	AJF.PhotoMover.Service.exe install

	Du burde nu kunne finde service i Service-vinduet under Kontrolpanel.

	Service kan startes fra Service-vinduet eller fra kommando-prompten ved at skrive
	net start ajf.photomover

	Hvis du senere ønsker at afinstallere skal du i stedet skrive
	>AJF.PhotoMover.Service.exe uninstall

5. Check i logfilen at service kører som forventet.