# ToDe - Tower Defense hra v Monogame (C#)

Průběžně aktualizovaný zdrojový kód ke hře Tower Defense vyvíjené na _let's program_ streamu v Monogame a jazyce C#.
* Živé streamy na *Twitch* kanále [programko.net/tode/tw](https://www.twitch.tv/petrvobornik) &nbsp; [![Twitch Status](https://img.shields.io/twitch/status/petrvobornik?label=Twitch&style=social)](https://www.twitch.tv/petrvobornik)
* Archiv streamů v *YouTube* playlistu [programko.net/tode/yt](https://www.youtube.com/playlist?list=PLxTqV9i8bnb_BN9k_-W_CjXfumNzqiPEe) &nbsp; [![YouTube Channel Subscribers](https://img.shields.io/youtube/channel/subscribers/UCpa5E1uKjQ0gGlui9G1wPCg?label=YouTube&style=social)](https://www.youtube.com/user/VobornikPetrCZ/)
* Možnost diskuze k projektu mimo stream na *Discord* serveru [programko.net/tode/dc](https://discord.gg/GVgz2a3qd8) &nbsp; [![Discord](https://img.shields.io/discord/836206232485232700?color=%237289DA&label=Discord&logo=discord&logoColor=%23FFF&style=plastic)](https://discord.gg/GVgz2a3qd8)
* A URL alias k tomuto *GitHub* repozitáři je [programko.net/tode/gh](https://github.com/PetrVobornik/ToDe)

## Roadmap
* ~~Projekt s NuGety (bez šablony) s VS kompilovaným obsahem~~
* ~~Vykreslování mapy~~
* ~~Průchod nepřátel po automaticky vypočtené cestě~~
* ~~Nabídka a umísťování věží~~
* ~~Otáčení a střelba věží~~
* ~~Načítání parametrů levelů a vln ze souboru~~
* ~~Tankové jednotky~~
* ~~Zvuky~~
* ~~Pauza~~
* ~~Prohra (GameOver)~~
* ~~Načtení 2 a dalších levlů (výhra)~~
* ~~Přepínání textur u levelů~~
* ~~Další typy jednotek~~
* ~~Upravit texturu, aby každá dlaždice měla okolo sebe z každé strany 1px navíc~~
* ~~Postupně zpřístupňované věže (počty) / ekonomika~~
* ~~Ukazatel zdraví nepřátel~~
* ~~Zdraví nepřátel se pomalu obnovuje~~
* ~~Při prohře opakovat kolo, ne celou hru~~
* ~~Transpozice mapy při překlopení displeje~~
* ~~Lepší ovládací prvek pro panel nabídky~~
* ~~Možnost zbořit věž~~
* ~~Překážky na ploše (+ možnost je za $ odstranit)~~
* ~~Možnost upgrade věží, možnost definovat i parametry raket~~
* ~~Možnost nějak zobrazit dosah věží~~
* ~~Editor levelů~~
* ~~Možnost kola a jejich sady sdílet~~
* ~~Ukládat nastavení editoru do Preferences~~
* ~~Možnost v XML levelu definovat úplně vše (včetně všech vlastností věží ve všech levelech, i dostupnost jednotlivých typů)~~
* Vyřešit možnost opětovného zapnutí hry bez nutnosti vypínat aplikaci (v UWP i na Androidu)
* V ovládacím panelu nabízet pouze ty věže, na které level podporuje
* Import souborů s levely
* Možnost větších levelů - jejich posouvání a zoomování dotyky (ale bez ovládacího panelu)
* Pozadí u panelu s nabídkou
* Menu s dalšími možnostmi (např. restart levelu, konec hry, hlasitost...)
* Další typy věží
* Dvě střílny
* Letadlo pouští parašutisty v půlce cesty
* Protiútok jednotek (tanky střílí na věže, nálety...)
* Mapa levelů
* Zvyšovat max. dostupný level věží nákupy mimo levley za ušetřené a vyhrané $
* Jiná textura na explozi
* Ozdobné okraje cesty
* Lepší SplashScreen
* Zvuky pro umístění věže, zboření věže, upgrade věže, zboření překážky...
* Hudba (doporučené zdroje: [gamesounds](https://gamesounds.xyz), [game-resources](https://blog.felgo.com/game-resources/free-music-for-games))
* Store
* Grafický editor levelů
* Cloudové počítání top skóre za daný level/sadu


<p align="center">
<img src="https://github.com/PetrVobornik/ToDe/blob/main/Soubory/texture-sample.png?raw=true" alt="Ukázka použité textury" />
</p>


## Otevření projektu
Stáhněte si celý repozitář k sobě na počítač. Otevřete ToDe.sln ve Visual Studiu 2022. 
Otevřete okno terminálu (Ctrl+;), a zadejte následující příkazy, které nainstalují a zaregistrují MGCB (MonoGame Content Builder) kompilátor obsahu (zdrojových obrázků, zvuků atd.):
* dotnet tool install -g dotnet-mgcb-editor
* mgcb-editor --register


## Použité zdroje
* [Monogame](https://www.monogame.net)
* Textura: Kenney Tower Defense assets: [opengameart.org](https://opengameart.org/content/tower-defense-300-tilessprites), [kenney.nl](https://www.kenney.nl/assets/tower-defense-top-down)
* Nástroj na úpravu textur: [SpriteSheetPacker](https://github.com/PetrVobornik/SpriteSheetPacker), upraveno z originálu [Kelly Gravelyn](https://github.com/kellygravelyn/SpriteSheetPacker)
* [Koš](https://www.freeiconspng.com/img/28675)
* Inspirace projekty
    * [Hra Asteroidy](https://github.com/PetrVobornik/prednasky/tree/master/Xamarin.Forms/09-Hra)
    * [Herní smyčka ve WPF](https://www.youtube.com/playlist?list=PLxTqV9i8bnb_jTFqFLAE2cnB6ec6u6N5T)
    * [Platformer 2D](https://github.com/MonoGame/MonoGame.Samples/tree/develop/Platformer2D/)
* Zvuky
    * [Vypuštění rakety](https://freesound.org/people/jorgerosa/sounds/458669/)
    * [Exploze rakety](https://freesound.org/people/derplayer/sounds/587194/)
    * [Střelba](https://freesound.org/people/timgormly/sounds/170167/)
    * [Otáčení věže](https://freesound.org/people/KieranKeegan/sounds/418881/)
    * [Výhra](https://freesound.org/people/LittleRobotSoundFactory/sounds/270402/)
    * [Prohra](https://freesound.org/people/LittleRobotSoundFactory/sounds/270466/)


## Další odkazy
* Twitter: [@VobornikPetr](https://twitter.com/VobornikPetr) &nbsp; [![Twitter Follow](https://img.shields.io/twitter/follow/VobornikPetr?label=Twitter&style=social)](https://twitter.com/VobornikPetr)
* YouTube: [Programko.NET](http://programko.net) &nbsp; [![YouTube Channel Subscribers](https://img.shields.io/youtube/channel/subscribers/UCpa5E1uKjQ0gGlui9G1wPCg?label=YouTube&style=social)](https://www.youtube.com/user/VobornikPetrCZ/)
* GitHub: [ToDe](https://github.com/PetrVobornik/ToDe)
* Web: [petrvobornik.cz](https://www.petrvobornik.cz)
* Podpora: [programko.net/podpora](https://programko.net/podpora)