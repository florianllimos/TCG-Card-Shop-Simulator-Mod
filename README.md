# Mod TCG Card Shop Simulator (BepInEx)

Ce mod ajoute :
- `F9` : ajoute `100000` dollars.
- `F10` : tente de passer au jour suivant.

## 1) Pré-requis

- Jeu **TCG Card Shop Simulator** sur PC.
- **BepInEx 5 x64 (Mono)** installé dans le dossier du jeu.
- **.NET SDK** (pour compiler le `.csproj`).

## 2) Préparer les DLL de référence

Copie ces DLL dans le dossier :
- `libs/BepInEx/BepInEx.dll`
- `libs/BepInEx/UnityEngine.dll`

Tu peux les prendre depuis ton installation BepInEx + dossier du jeu.

## 3) Compiler

Depuis la racine du workspace :

```powershell
dotnet build .\src\TCGCardShopHotkeys\TCGCardShopHotkeys.csproj -c Release
```

La DLL compilée sera dans :
- `src/TCGCardShopHotkeys/bin/Release/net472/TCGCardShopHotkeys.dll`

## 4) Installer le mod

Copie la DLL compilée dans :
- `<DossierDuJeu>\BepInEx\plugins\TCGCardShopHotkeys.dll`

Puis lance le jeu.

## 5) Utilisation

- `F9` : ajoute 100000.
- `F10` : passe au jour suivant.

Les logs sont visibles dans :
- `<DossierDuJeu>\BepInEx\LogOutput.log`

## Notes importantes (version Xbox App / Game Pass)

Sur certaines installations Xbox App (permissions/sandbox), le chargement de mods peut être bloqué.
Si BepInEx ne se charge pas, il faut :
- vérifier que l’installation du jeu autorise l’accès complet aux fichiers,
- ou utiliser une version du jeu qui accepte l’injection BepInEx.
