# Media Notes Pro

Plugin backend-first para notas com timestamp no Jellyfin.

## Recursos

- `GET /MediaNotes/{itemId}`, `POST /MediaNotes/{itemId}` e `GET /MediaNotes/{itemId}/Export`.
- Notas por usuário e mídia, JSON com escrita atômica.
- Modal Web com timestamp, autosave e exportação.
- Página mobile/WebView com `itemId` e `positionTicks`.
- Workflow GitHub Actions.

## Limitação importante

Plugins de servidor não injetam JavaScript automaticamente no Jellyfin Web nem adicionam botões aos apps oficiais Android/iOS. `IHasWebPages` apenas registra páginas/assets. No Web, carregue os assets numa build customizada ou no `index.html`. No mobile, altere o cliente para abrir a WebView ou consumir a API. O backend é único e sincroniza as duas interfaces.

## Build e deploy

Requer .NET 9. Compile usando a mesma versão do servidor:

```powershell
.\build.ps1 -JellyfinVersion 10.11.11
```

As referências `Jellyfin.Common`, `Jellyfin.Controller` e `Jellyfin.Model` fornecem também os namespaces/assemblies MediaBrowser. Não copie essas DLLs. Ajuste `targetAbi` no `meta.json` para seu servidor.

Copie `MediaNotesPro.dll` e `meta.json` para `/var/lib/jellyfin/plugins/MediaNotesPro/` (Linux), `/config/plugins/MediaNotesPro/` (Docker) ou `%ProgramData%\Jellyfin\Server\plugins\MediaNotesPro\` (Windows), e reinicie.

## Web

```html
<link rel="stylesheet" href="/web/MediaNotesPro.css">
<script src="/web/MediaNotesPro.js"></script>
```

Para integração estável, chame `MediaNotesPro.configure({apiClient, getItemId, getPositionSeconds})`.

## Android/iOS

Abra uma WebView autenticada:

```
{serverUrl}/web/MediaNotesProMobile.html?itemId={ItemId}&positionTicks={PlaybackPositionTicks}
```

Ou use a API com `X-Emby-Token`. Para a share sheet, baixe `/MediaNotes/{itemId}/Export`.
