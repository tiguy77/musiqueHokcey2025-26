# Aréna DJ

Application Windows pour piloter les musiques d'un match de hockey. Lorsque le cloud est configuré, l'accès est protégé par une connexion et le bouton **Télécharger les musiques** synchronise la bibliothèque cloud vers le poste afin que la lecture soit locale pendant le match. Sans base de données configurée, l'application démarre directement en **mode local** avec les pistes déjà présentes.

## Console de match

- **Fin de partie** : lit les MP3 de la catégorie `finGame`, indépendamment des entractes; plusieurs pistes sont lues à tour de rôle à chaque nouvelle pression. Appuyer pendant la lecture arrête la piste.
- **Reset** : arrête la lecture, remet les listes au départ, désélectionne les équipes et réactive les commandes. Il ne supprime ni les MP3 ni les préférences personnelles, et ne déconnecte pas l'utilisateur.
- **EN LECTURE / MUSIQUE SUIVANTE** : panneau au bas de la fenêtre indiquant le morceau actif et le prochain morceau de la liste concernée.

## Personnaliser (nouvelle section)

Ouvrez **Personnaliser** dans le coin supérieur droit de la console de match. Cet écran permet de gérer les pistes et les équipes sans modifier le code C#.

Dans **Bibliothèque musicale**, sélectionnez d'abord la catégorie correspondant au bouton voulu : Musique générale (`all`), Entracte (`entracte`), Fin de partie (`finGame`), Pénalité locale (`PPLocal`), Pénalité visiteur (`PPvis`), Musiques de but (`buts`) ou Échauffement (`warmup`). Cliquez sur **Importer des MP3** pour copier un ou plusieurs fichiers dans cette catégorie. Vous pouvez modifier leur titre affiché, déplacer les musiques importées entre catégories et supprimer une importation après confirmation. Le programme ne supprime pas les fichiers de la bibliothèque historique : pour les classer ailleurs, importez-en une copie dans la catégorie voulue.

Dans **Équipes et musiques de but**, choisissez une équipe, modifiez son nom et sélectionnez ses musiques de but et d'échauffement dans les deux catégories correspondantes. Cliquez sur **Enregistrer cette équipe** pour confirmer la modification. Les boutons BUT LOCAL et BUT VISITEUR utiliseront la musique de l'équipe sélectionnée; ÉCHAUFFEMENT utilisera celle de l'équipe locale. Les modifications ne changent pas les noms ni les titres dans la base cloud.

Les musiques importées sont copiées dans `%LOCALAPPDATA%\ArenaDJ\musiques\<catégorie>\` et les préférences sont enregistrées dans `%LOCALAPPDATA%\ArenaDJ\preferences.json`. Ces données appartiennent à votre profil Windows et survivent à une nouvelle publication de l'exécutable; effectuez toutefois une sauvegarde de ce dossier avant de changer d'ordinateur. Les fichiers MP3 de l'ancienne bibliothèque, situés à côté de l'exécutable dans `netX\musiques`, restent également disponibles. **Télécharger les musiques** continue de gérer uniquement la bibliothèque cloud : importer un MP3 local ne le téléverse pas vers Neon ou un stockage en ligne.

L'ouverture de Personnaliser réinitialise le match et arrête la lecture en cours après confirmation afin de permettre le déplacement ou la suppression de fichiers audio. La réinitialisation ne supprime jamais les préférences enregistrées.

## Configuration sécurisée

1. Révoquez tout mot de passe de base de données publié par erreur. Ne placez jamais l'URL PostgreSQL dans le code.
2. Pour activer la connexion et la synchronisation cloud, définissez la chaîne Neon uniquement sur le poste qui exécute l'application :

```powershell
$env:MUSIQUE_HOCKEY_DATABASE_URL = "postgresql://UTILISATEUR:MOT_DE_PASSE@HOTE/neondb?sslmode=require"
```

Au premier démarrage, l'application crée les tables `app_users` et `music_tracks`. Pour créer le premier utilisateur, générez un hash PBKDF2 compatible avec `AuthService.HashPassword`, puis insérez `id`, `email`, `display_name` et `password_hash` dans `app_users`. Les mots de passe ne sont pas stockés en clair.

## Stockage des musiques dans le cloud

PostgreSQL/Neon convient aux métadonnées et aux comptes, pas aux MP3 eux-mêmes. Utilisez un stockage objet (Cloudflare R2, Amazon S3, Azure Blob ou Supabase Storage) et enregistrez une URL HTTPS dans `music_tracks.download_url`.

```sql
INSERT INTO music_tracks (title, category, download_url, file_name)
VALUES ('Musique exemple', 'all', 'https://cdn.exemple.ca/musiques/exemple.mp3', 'exemple.mp3');
```

Catégories cloud utilisées : `all`, `warmup`, `buts`, `PPLocal`, `PPvis`, `entracte`, `finGame`. Pour du contenu privé ou licencié, privilégiez des URL signées à courte durée émises par une API plutôt que des fichiers publics.

## Développement et publication Windows

```bash
dotnet restore MusiqueHockey/MusiqueHockey.sln
dotnet build MusiqueHockey/MusiqueHockey.sln
```

Depuis la racine du dépôt, fermez les instances de l'application et publiez une nouvelle copie avec :

```powershell
.\publish-windows.ps1
```

Démarrez ensuite `dist\windows-x64\MusiqueHockey.exe` et actualisez votre raccourci Windows. L'ancien exécutable ne se met pas à jour tout seul. Le script efface le dossier `dist\windows-x64` avant publication, mais ne touche pas à la bibliothèque personnelle stockée dans `%LOCALAPPDATA%\ArenaDJ`. L'exécutable publié inclut le runtime .NET nécessaire.
