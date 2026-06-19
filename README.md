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
<link rel="stylesheet" href="/web/configurationpage?name=MediaNotesPro.css">
<script src="/web/configurationpage?name=MediaNotesPro.js"></script>
```

Para integração estável, chame `MediaNotesPro.configure({apiClient, getItemId, getPositionSeconds})`.

## Android/iOS

Abra uma WebView autenticada:

```
{serverUrl}/web/configurationpage?name=MediaNotesProMobile&itemId={ItemId}&positionTicks={PlaybackPositionTicks}
```

Ou use a API com `X-Emby-Token`. Para a share sheet, baixe `/MediaNotes/{itemId}/Export`.

## Releases e repositório Jellyfin

O workflow `.github/workflows/publish.yml` publica uma release ao receber uma tag que corresponda à versão do `meta.json`:

```bash
git tag v2.0.0.0
git push origin v2.0.0.0
```

Depois, cadastre no Jellyfin:

```text
https://raw.githubusercontent.com/SEU-USUARIO/MediaNotesPro2.0/main/manifest.json
```

Substitua o usuário/repositório. Em **Settings → Actions → General → Workflow permissions**, habilite **Read and write permissions**, pois o workflow atualiza o `manifest.json`.
