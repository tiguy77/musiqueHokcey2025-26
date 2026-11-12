# Candidature SignPath Foundation — Aréna DJ

## Statut réel

Dossier préparé, non soumis. Le projet n'est pas encore accepté par SignPath. L'installateur public v2.0.5 est non signé. Ce document n'atteste ni d'une certification de sécurité ni de l'absence de logiciels malveillants. L'ajout de la licence ne signe pas les exécutables déjà publiés.

## Informations à reprendre dans la demande

- Project name: Aréna DJ
- Source repository: https://github.com/tiguy77/musiqueHokcey2025-26
- Project documentation: https://github.com/tiguy77/musiqueHokcey2025-26#readme
- License: MIT, pour le code original du projet; dépendances sous leurs licences respectives.
- License URL: https://github.com/tiguy77/musiqueHokcey2025-26/blob/main/LICENSE
- Existing releases: https://github.com/tiguy77/musiqueHokcey2025-26/releases
- Permanent installer URL: https://github.com/tiguy77/musiqueHokcey2025-26/releases/latest/download/ArenaDJ-Setup.exe
- Platform: Windows 10/11, x64 application, .NET 8 Windows Forms, self-contained deployment.
- Installer: Inno Setup, ArenaDJ-Setup.exe.
- Build workflow: .github/workflows/release-windows.yml
- Build script: publish-windows.ps1
- Runtime dependencies declared by the project: NAudio 2.2.1, Npgsql 8.0.6, .NET runtime. Leurs licences et celles des dépendances transitives doivent être conservées et vérifiées avant soumission.

### Description proposée pour le formulaire

Aréna DJ is a Windows desktop application for controlling music during hockey games. It provides playback controls for goals, penalties, intermissions, warmups and the end of a game. Users can import their own MP3 files and customize teams and their goal and warmup tracks. The application supports local playback and optional user-configured cloud library synchronization. Source code and Windows installer releases are publicly available on GitHub. We request free code signing for the original application executable and its Windows installer, built from this repository through GitHub Actions.

### Demande proposée

We would like to apply to the SignPath Foundation open-source program. The original project code is licensed under MIT. We want to keep distributing ArenaDJ-Setup.exe through GitHub Releases and add a verifiable signing process with explicit release approval. We understand that eligibility and acceptance are determined by SignPath Foundation and that code signing does not guarantee immediate Microsoft SmartScreen reputation. Please advise on the required artifact configuration for signing our original application executable and Inno Setup installer without re-signing third-party binaries.

## Vérifications à terminer par le mainteneur avant envoi

- [ ] Utiliser un compte personnel de contact dans le formulaire officiel; ne pas publier ses coordonnées dans ce dossier.
- [ ] Confirmer les droits sur le code original et examiner les composants tiers et dépendances transitives. Le fait qu'un dépôt soit public ne suffit pas à établir ces droits.
- [ ] Confirmer l'absence de code propriétaire incompatible ou de double licence commerciale exclue par le programme.
- [ ] Activer l'authentification multifacteur pour GitHub et SignPath, pour toutes les personnes participant à la signature.
- [ ] Confirmer les rôles proposés : auteur, relecteur et approbateur — tiguy77. Les règles de revue des contributions externes et d'approbation manuelle de chaque signature doivent être respectées.
- [ ] Documenter et vérifier le comportement réseau et la politique de confidentialité, y compris le mode cloud optionnel et ses services tiers. Ne pas déclarer que l'application est entièrement hors ligne : la documentation décrit une connexion et une synchronisation cloud lorsqu'elles sont configurées.
- [ ] Vérifier que les métadonnées de produit et de version des fichiers originaux sont cohérentes avant la configuration des restrictions SignPath.
- [ ] Lire et accepter personnellement les conditions en vigueur : https://signpath.org/terms

Le contenu musical importé par les utilisateurs n'est pas concédé sous MIT par le projet; chaque utilisateur doit détenir les droits nécessaires. La licence du code ne donne aucun droit supplémentaire sur les musiques, marques ou contenus de tiers.

## Déposer la candidature

Ouvrir https://signpath.org/apply et reprendre les informations ci-dessus après avoir terminé les vérifications. Le propriétaire doit utiliser son propre compte et ses propres coordonnées. Aucun compte SignPath n'est créé et aucune demande n'est envoyée automatiquement par ce document. L'acceptation dépend notamment de l'éligibilité et de la réputation vérifiable du projet; elle n'est pas garantie.

## Code signing policy — cible après acceptation

Cette section décrit le processus visé, pas un service déjà actif.

1. Construire les binaires depuis le code source du dépôt via GitHub Actions et conserver une provenance vérifiable.
2. N'envoyer à la signature que les exécutables originaux autorisés et l'installateur; ne pas re-signer les DLL ou exécutables tiers sous l'identité du projet.
3. Faire approuver explicitement chaque demande de signature par le mainteneur habilité.
4. Signer et horodater l'exécutable original avant de construire l'installateur, puis signer et horodater l'installateur. Définir avec SignPath le traitement du désinstallateur Inno Setup et les restrictions de métadonnées nécessaires.
5. Vérifier sous Windows les signatures Authenticode et l'horodatage de l'application, de l'installateur et des autres exécutables originaux distribués; bloquer la publication en cas d'échec.
6. Tester l'installation, le démarrage et la désinstallation; ne pas modifier les fichiers après signature.
7. Après validation et accord de publication, distribuer une nouvelle version signée en conservant le nom ArenaDJ-Setup.exe et le lien permanent. Ne pas remplacer silencieusement les anciennes releases non signées.

Après acceptation seulement, ajouter sur les pages d'accueil et de téléchargement la mention requise par SignPath : « Free code signing provided by SignPath.io, certificate by SignPath Foundation », avec les liens officiels, les rôles confirmés et la politique de confidentialité validée. Ne pas afficher cette mention comme un fait avant acceptation.

## Ce qui reste volontairement inchangé

Le site, son design, son numéro de version, son URL de téléchargement et les releases existantes. Le workflow de publication n'est pas modifié tant que le projet n'est pas accepté et que sa configuration SignPath n'est pas disponible. Aucun abonnement payant n'est activé. Aucun token, mot de passe, certificat privé ou secret ne doit être ajouté au dépôt ou transmis en clair dans une conversation.
