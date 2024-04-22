<div align="center">

![LiveMap](https://raw.githubusercontent.com/BillyGalbreath/VS-LiveMap/master/web/public/images/og.webp)

# LiveMap

[![Build Status](https://img.shields.io/jenkins/build?jobUrl=https%3A%2F%2Fci.pl3x.net%2Fjob%2FVS-LiveMap&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAACt0lEQVQ4y4WT3UuTURzHv8/Opm7PxrOpU+frnJM1p/kylShQb3ojEeyVoqIuI5D+gKCCwosgMLoW8SYipChiXllIkJiZG5ot3fJ1L24+29x0bs/2nC4icFPpd/XjnM/58ON7+DE4oLq6upTlOt1gkTqvud5kKA5HtkKOH85nQ6/eDWSz0uwDq9UqM1RVuc90NDGnjjcUplMJECJR2edrHkW3YukR29iLvbwkW2DU17ytqKjUlihzCiZnfiK6HQcAmKp1XHuz+Wo2nyG4bTS2aFKJ077fbpTqNEx7Yw1cK37EtuNY8QbASkjbSbO59jABsSjk71vr9aTRWIy0kAYAVJUVQskq4PUHodcoZY0MBve+I/+aviN1Dy6q5N1eCcNYOttgMJT+vaDA/OIqtmLbEJbW0OoLlPOsct25yU9nTFCZI+kmANPmWsbC1ByEtAg+EsVmZAvfZ3+hqIBDbM4NtUQiMUhJ575fkDGMnIKCFylGPs9BU12FPJaApkVYdFoMDLxGOk3QDEBJSOE+QUwUAzYqw2iJEQlOi092L4yOCSh347ADuMkQPNHV45tvFgKl8X0hupIpe4hIIXBa7EbDGF8PwS8SxFRqmHovwGa0QB72oYkR4RGoa1+IORznOsGIl+dZjTIJBlIpgTOPA82TQSZXYSMhQBP0IQgxOCkId9YCAT5DsB4K8Vw+J4+z+V0JkssUJcO41GSCvqIUX+3TCIcjMLccpTf6+1kfz0e+TEx8zBAAwGyQH9ca6u7SnFy2t8GEvscPYe3sQKmag8PpSp0730NYhSIZDoV2bKOjLw/chZ2w/ylC/itjJF59D8gHAFl+weaSZ3V5aHj4fkVZSe2Ca+kN/lfHWlt7fF5vklJKAxsbi7euX/twEEcOE6x5PAucSnXW6/HmOmZm6OTU1HOX2+3I5v4AuDsT2N5aTFUAAAAASUVORK5CYII=)](https://ci.pl3x.net/job/VS-LiveMap/)
[![Downloads](https://img.shields.io/endpoint?url=https://vs.pl3x.net/shields.io/vintagestory/livemap)](https://mods.vintagestory.at/livemap)
[![Join us on Discord](https://img.shields.io/discord/171810209197457408.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/JXra7N4)

[![MIT License](https://img.shields.io/github/license/BillyGalbreath/VS-LiveMap?&logo=github&color)](LICENSE)
[![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/BillyGalbreath/VS-LiveMap?logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8%2F9hAAABhWlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw0AcxV9bxSIVh1YQcYhQnSyIiuimVShChVArtOpgcukXNGlIUlwcBdeCgx%2BLVQcXZ10dXAVB8APE0clJ0UVK%2FF9SaBHjwXE%2F3t173L0D%2FPUyU82OMUDVLCOViAuZ7KrQ9YogwujDEGYkZupzopiE5%2Fi6h4%2BvdzGe5X3uz9Gj5EwG%2BATiWaYbFvEG8dSmpXPeJ46woqQQnxOPGnRB4keuyy6%2FcS447OeZESOdmieOEAuFNpbbmBUNlXiSOKqoGuX7My4rnLc4q%2BUqa96TvzCU01aWuU5zEAksYgkiBMioooQyLMRo1UgxkaL9uId%2FwPGL5JLJVQIjxwIqUCE5fvA%2F%2BN2tmZ8Yd5NCcaDzxbY%2FhoGuXaBRs%2B3vY9tunACBZ%2BBKa%2FkrdWD6k%2FRaS4seAb3bwMV1S5P3gMsdoP9JlwzJkQI0%2Ffk88H5G35QFwrdA95rbW3Mfpw9AmrpK3gAHh8BIgbLXPd4dbO%2Ft3zPN%2Fn4Ax9dyyerighsAAAAGYktHRAAAAAAAAPlDu38AAAAJcEhZcwAADdcAAA3XAUIom3gAAAAHdElNRQfmCBMVKAA5pS6%2BAAABlElEQVQ4y82PP2gVQRDGf7N3t%2Bvdixpi0N5OELFKJ1iohBciKlgYJLX6YkBbC0sVooVFBAvBPw%2BFZzrJs7DR2iYHRhBsxNI8VLwUx92MRXJGxKCp9AfL7DfDfPutFO3z5wy5DuRlWU2OvLj7hduLYXh0ZSEkOh4SjUKiBK%2BEZP34Gu%2FtbebLE86Qa8BO4FDwyWmAbPjzMWACiNgEMdun6macwfJ6z2qxZYBI6ndAxR%2BRN%2FL1ZGeXlDqFkm%2Fv33nZjHZ0u2OZrw%2F7pBYf16Re8UEJ8VpNE33fP3BxgX%2BOFOOdtjmuGpoPtT51pNcrMZORx4%2FmslQnslAlWahItymZrz%2Bmqc4%2B2z%2B71BjE5uwesEeQsaLY%2FQp42LrfPUqwy2DNO03ZK9hN4Ehj4IDBjzjKCoC5aMDG9q%2BhBz%2BrWCN3KqptBtG89Xx%2BEWB1%2Bszr8OTBFMgkSLKWQAA%2BVCU3%2BK%2BQb%2B0LB4FLGHmrP39LNv3773Ei9IBphLnVduf4VhM4M9JGqGzc%2F5bYnDsrqlcQloaK0adbNfgOUn6NRlZZ46YAAAAASUVORK5CYII%3D)](https://www.codefactor.io/repository/github/BillyGalbreath/VS-LiveMap)
[![Lines of Code](https://img.shields.io/endpoint?url=https%3A%2F%2Fvs.pl3x.net%2Fshields.io%2Floc.php)]()

<!--[![Servers](https://img.shields.io/bstats/servers/10133)](https://bstats.org/plugin/bukkit/Pl3xMap/10133)-->
[![Stargazers](https://img.shields.io/github/stars/BillyGalbreath/VS-LiveMap?style=flat&label=stars&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABhWlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw0AcxV9bxSIVh1YQcYhQnSyIiuimVShChVArtOpgcukXNGlIUlwcBdeCgx+LVQcXZ10dXAVB8APE0clJ0UVK/F9SaBHjwXE/3t173L0D/PUyU82OMUDVLCOViAuZ7KrQ9YogwujDEGYkZupzopiE5/i6h4+vdzGe5X3uz9Gj5EwG+ATiWaYbFvEG8dSmpXPeJ46woqQQnxOPGnRB4keuyy6/cS447OeZESOdmieOEAuFNpbbmBUNlXiSOKqoGuX7My4rnLc4q+Uqa96TvzCU01aWuU5zEAksYgkiBMioooQyLMRo1UgxkaL9uId/wPGL5JLJVQIjxwIqUCE5fvA/+N2tmZ8Yd5NCcaDzxbY/hoGuXaBRs+3vY9tunACBZ+BKa/krdWD6k/RaS4seAb3bwMV1S5P3gMsdoP9JlwzJkQI0/fk88H5G35QFwrdA95rbW3Mfpw9AmrpK3gAHh8BIgbLXPd4dbO/t3zPN/n4Ax9dyyerighsAAAAGYktHRAAAAAAAAPlDu38AAAAJcEhZcwAADdcAAA3XAUIom3gAAAAHdElNRQfmCBMVNjtc7/hFAAABIElEQVQ4y62SzS5DURSFv6smXkAUCRU0UdKYGNTPyCsYYOYFGGi8Ao9QM0PxCh6CgQ4qfiLpBFEjdKCfySaXtDch1uScs9Ze62TvcyAD6o66zV+gjqpvalsd61XXl5GxBySx3/3t7UPqi1pTD9VXdaRbbZIyDQLTwBSwBqzGGaABnAInwCXQSJLk/tO4orb8jra6nwo/CC6NlrqMOq421Y5aVSfUXJe2cqFVo7b5NdwIuVaf1IWM2cyrD+qdOvlTLERIS53pYi6FdqMWet2wGP1tdNE2Q1vK+gfDsdbDlFfzwV3Ems8KmAXegcd4hSvgVq0Bz6GV0ob+HgF1YAA4Cn4LWA9tLusHnscTHavFFF8MrqOeZQVU1HKGXlYr/Cc+AKuOI2h/Jrf7AAAAAElFTkSuQmCC)](https://github.com/BillyGalbreath/VS-LiveMap/stargazers)
[![Forks](https://img.shields.io/github/forks/BillyGalbreath/VS-LiveMap?style=flat&label=forks&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABhWlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw0AcxV9bxSIVh1YQcYhQnSyIiuimVShChVArtOpgcukXNGlIUlwcBdeCgx+LVQcXZ10dXAVB8APE0clJ0UVK/F9SaBHjwXE/3t173L0D/PUyU82OMUDVLCOViAuZ7KrQ9YogwujDEGYkZupzopiE5/i6h4+vdzGe5X3uz9Gj5EwG+ATiWaYbFvEG8dSmpXPeJ46woqQQnxOPGnRB4keuyy6/cS447OeZESOdmieOEAuFNpbbmBUNlXiSOKqoGuX7My4rnLc4q+Uqa96TvzCU01aWuU5zEAksYgkiBMioooQyLMRo1UgxkaL9uId/wPGL5JLJVQIjxwIqUCE5fvA/+N2tmZ8Yd5NCcaDzxbY/hoGuXaBRs+3vY9tunACBZ+BKa/krdWD6k/RaS4seAb3bwMV1S5P3gMsdoP9JlwzJkQI0/fk88H5G35QFwrdA95rbW3Mfpw9AmrpK3gAHh8BIgbLXPd4dbO/t3zPN/n4Ax9dyyerighsAAAAGYktHRAAAAAAAAPlDu38AAAAJcEhZcwAADdcAAA3XAUIom3gAAAAHdElNRQfmCBMVNCYN3/YeAAAA/UlEQVQ4y7WTQUoDQRBFf01czlJcxUyOINGjjAvFHMFzZGdygOwDwTtk6UZcqLlAxCAuMigug89FamIzdAIN+qGhq/6v6qrqbumvAJwBj8AHMAQs4DJgBHy65jSW4Bl4AaZsUAbcufumrnmquSzIcSzpTtLA7XbA1fuBa9qxCob8YgUUAdcFqoC/iSXIgLELOhG+49w4nM+2BTP7ljR3M4/MufbNzYxdN1E0Sm2ialZnsVIllZKOJF24eyLpXdKtmS1S3sYMmO3THOwJziUdbrbkZvaVcnILeAh6vweylAQ9D7z2BXCS0sJS0lrSpdtrSW+pn6sPLIFX4Er/hR9C0wl1FTBzNwAAAABJRU5ErkJggg==)](https://github.com/BillyGalbreath/VS-LiveMap/network/members)
[![Watchers](https://img.shields.io/github/watchers/BillyGalbreath/VS-LiveMap?style=flat&label=watchers&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABhWlDQ1BJQ0MgcHJvZmlsZQAAKJF9kT1Iw0AcxV9bxSIVh1YQcYhQnSyIiuimVShChVArtOpgcukXNGlIUlwcBdeCgx+LVQcXZ10dXAVB8APE0clJ0UVK/F9SaBHjwXE/3t173L0D/PUyU82OMUDVLCOViAuZ7KrQ9YogwujDEGYkZupzopiE5/i6h4+vdzGe5X3uz9Gj5EwG+ATiWaYbFvEG8dSmpXPeJ46woqQQnxOPGnRB4keuyy6/cS447OeZESOdmieOEAuFNpbbmBUNlXiSOKqoGuX7My4rnLc4q+Uqa96TvzCU01aWuU5zEAksYgkiBMioooQyLMRo1UgxkaL9uId/wPGL5JLJVQIjxwIqUCE5fvA/+N2tmZ8Yd5NCcaDzxbY/hoGuXaBRs+3vY9tunACBZ+BKa/krdWD6k/RaS4seAb3bwMV1S5P3gMsdoP9JlwzJkQI0/fk88H5G35QFwrdA95rbW3Mfpw9AmrpK3gAHh8BIgbLXPd4dbO/t3zPN/n4Ax9dyyerighsAAAAGYktHRAAAAAAAAPlDu38AAAAJcEhZcwAADdcAAA3XAUIom3gAAAAHdElNRQfmCBMVNw4TRw0nAAAA3UlEQVQ4y83SP04CURAG8I0lewHOwAFUaiwkdmAlp8CL4FHopfIvtOIJWE3opIBK489mQPKy6xYWOskkL9/MN/PNzMuyf2fIcYkZVuGzwPI68gle8Yl7jMIfAntBp4o8wAeecFgSP8I8cgZp8DwC12j8oLCBCd7R34ItbHCzT8ZZSC7QTYrcYo1WhjGWaCbdCt+2SGLN4IwPfnu07QjrkhG6oWKB0+TMd7sRAuzHYuqWmO8tsVd1xjmOS8htPEfORVWHTmweHnEVPg2sqPxIicxhFFjhLd7D2q/8J/YFHSJt9VSqQ08AAAAASUVORK5CYII=)](https://github.com/BillyGalbreath/VS-LiveMap/watchers)

<br>

<big><b>LiveMap is a browser-based world map viewer for Vintage Story servers</b></big>

</div>

## Features

* *todo*

## Demo

A live demo of LiveMap can be accessed at:

[https://vs.pl3x.net/map](https://vs.pl3x.net/map)

<img src="https://mods.vintagestory.at/files/asset/11347/markers.png" alt="Screenshot of markers on map" width="500">

## Downloads and Releases

All releases can be downloaded from the VintageStory ModDB site at:

[https://mods.vintagestory.at/livemap](https://mods.vintagestory.at/livemap)

## Support the Developer

If you enjoy this mod, please consider supporting the developer. A significant amount of time has gone into this project. Donations encourage the continuation to do so.

https://github.com/sponsors/BillyGalbreath

https://ko-fi.com/BillyGalbreath

https://patreon.com/BillyGalbreath

## Developers

There is an extensive marker API that allows you to automate adding/updating your own layers and markers on the map.

https://billygalbreath.github.io/VS-LiveMap

## Building from Source

Prerequisites:
* [.NET SDK 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* [npm](https://www.npmjs.com/)
* [Node.js](https://nodejs.org/en)

To build the project run the following command:

```
dotnet build
```

And the final zip will be located at `bin/mods/livemap.zip` if there were no errors.
