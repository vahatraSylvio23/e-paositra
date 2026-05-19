# Cahier des charges — Application de gestion de courrier

## 1. Contexte du projet

Le projet consiste à développer une application web de gestion de courrier avec **ASP.NET Core** et **Entity Framework Core**.  
Cette application permettra d’enregistrer, de suivre, de traiter et d’archiver les courriers entrants et sortants d’une organisation.  
L’objectif est de centraliser les informations pour faciliter le classement, la recherche et le suivi des documents. [cite:1][cite:3]

## 2. Objectifs du projet

- Enregistrer les courriers entrants et sortants.
- Suivre l’état de traitement de chaque courrier.
- Affecter un courrier à un service ou à un utilisateur.
- Rechercher rapidement un courrier selon plusieurs critères.
- Archiver les courriers traités. [cite:1][cite:3]

## 3. Périmètre fonctionnel

L’application couvrira les fonctions suivantes :

- Gestion des courriers entrants.
- Gestion des courriers sortants.
- Suivi du statut du courrier.
- Affectation à un service ou à un agent.
- Recherche par date, objet, expéditeur, destinataire ou statut.
- Archivage des courriers traités. [cite:1][cite:3]

## 4. Acteurs du système

### Administrateur
Il gère tous les paramètres de l’application, les utilisateurs et les courriers. [cite:1]

### Agent du courrier
Il enregistre les courriers, met à jour leur statut et suit leur traitement. [cite:1]

### Responsable de service
Il consulte les courriers affectés à son service. [cite:1]

### Utilisateur simple
Il peut consulter uniquement les courriers qui lui sont autorisés. [cite:1]

## 5. Besoins fonctionnels

- Créer, modifier, consulter et supprimer un courrier.
- Joindre éventuellement un fichier scanné au courrier.
- Affecter un courrier à un service.
- Mettre à jour le statut du courrier.
- Consulter l’historique des actions.
- Effectuer des recherches et des filtres avancés. [cite:1][cite:3]

## 6. Entités principales

Les principales entités du projet peuvent être :

- `Courrier`
- `Utilisateur`
- `Service`
- `TypeCourrier`
- `StatutCourrier`
- `HistoriqueCourrier` [cite:1][cite:3]

## 7. Exemple de structure des données

### Courrier
- Id
- Référence
- Objet
- DateRéception
- DateEnregistrement
- Expéditeur
- Destinataire
- TypeCourrierId
- StatutCourrierId
- ServiceId
- FichierJoint
- Observations [cite:1][cite:3]

### Utilisateur
- Id
- Nom
- Prénom
- Email
- MotDePasse
- Rôle
- ServiceId [cite:1]

### Service
- Id
- Nom
- Description [cite:1]

## 8. Contraintes techniques

- Développement en **ASP.NET Core MVC**.
- Utilisation de **Entity Framework Core** pour la gestion de la base de données.
- Architecture claire avec séparation des modèles, contrôleurs et vues.
- Base de données locale ou serveur SQL.
- Interface simple et responsive. [cite:1][cite:3]

## 9. Hors périmètre

- Signature électronique avancée.
- Workflow complexe de validation.
- Intégration avec un service postal externe.
- Paiement en ligne.
- Archivage légal certifié. [cite:1]

## 10. Livrables attendus

- Code source complet.
- Base de données créée avec migrations EF Core.
- Documentation technique.
- Cahier des charges.
- Diagramme de classes ou schéma de base de données. [cite:1][cite:3]

## 11. Conclusion

Cette application doit permettre une meilleure organisation du courrier au sein d’une structure.  
Elle doit être simple à utiliser, évolutive et adaptée à une gestion interne efficace. [cite:1][cite:3]
