# Aréna DJ

Application Windows moderne pour piloter les musiques d'un match de hockey. L'accès est protégé par une connexion et le bouton **Télécharger les musiques** synchronise en un clic la bibliothèque cloud vers le poste, afin que la lecture reste ensuite locale et fiable pendant le match.

## Configuration sécurisée

1. **Révoquez et régénérez immédiatement tout mot de passe de base de données publié dans une conversation ou un dépôt.** Ne placez jamais l'URL PostgreSQL dans le code.
2. Définissez la chaîne Neon uniquement sur le poste qui exécute l'application :

```powershell
$env:MUSIQUE_HOCKEY_DATABASE_URL = "postgresql://UTILISATEUR:MOT_DE_PASSE@HOTE/neondb?sslmode=require"
```

Au premier démarrage, l'application crée les tables `app_users` et `music_tracks`. Pour créer le premier utilisateur, générez un hash PBKDF2 compatible au moyen d'un petit outil d'administration appelant `AuthService.HashPassword`, puis insérez `id`, `email`, `display_name` et `password_hash` dans `app_users`. Les mots de passe ne sont jamais stockés en clair.

## Stockage des musiques

PostgreSQL/Neon convient aux **métadonnées** (titre, catégorie, URL, nom de fichier) et aux comptes, mais pas aux MP3 eux-mêmes. Utilisez un stockage objet (Cloudflare R2, Amazon S3, Azure Blob ou Supabase Storage) et enregistrez une URL HTTPS dans `music_tracks.download_url`.

```sql
INSERT INTO music_tracks (title, category, download_url, file_name)
VALUES ('Musique exemple', 'all', 'https://cdn.exemple.ca/musiques/exemple.mp3', 'exemple.mp3');
```

Catégories acceptées : `all`, `warmup`, `buts`, `PPLocal`, `PPVis`, `entracte`. Utilisez de préférence des URL signées à courte durée émises par une API plutôt que des fichiers publics pour du contenu privé ou licencié.

## Développement

```bash
dotnet restore MusiqueHockey/MusiqueHockey.sln
dotnet build MusiqueHockey/MusiqueHockey.sln
```
