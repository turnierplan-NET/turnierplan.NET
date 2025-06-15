export const de = {
  Application: {
    Name: 'turnierplan.NET'
  },
  General: {
    MatchOutcomeType: {
      AfterOvertime: 'n.V.',
      AfterPenalties: 'n.E.',
      SpecialScoring: '(§)'
    },
    DiscardChanges: 'Wenn Sie die Seite verlassen, werden Ihre Änderungen nicht gespeichert.',
    VersionBadge: 'Entwicklungsversion'
  },
  Footer: {
    LandingPage: 'Startseite',
    GitHub: 'GitHub'
  },
  Identity: {
    ChangePassword: {
      Title: 'Passwort ändern',
      OldPassword: 'Bisheriges Passwort:',
      NewPassword: 'Neues Passwort:',
      ConfirmNewPassword: 'Passwort bestätigen:',
      Failed: {
        EmptyOrExtraWhitespace: 'Geben Sie ein neues Passwort an, welches keine Leerzeichen am Anfang oder Ende hat.',
        PasswordsDoNotMatch: 'Die beiden Passwörter stimmen nicht überein.',
        InsecurePassword:
          'Das neue Passwort muss folgende Kriterien erfüllen:<ul><li>min. 10 Zeichen lang</li><li>min. 1 Großbuchstabe</li><li>min. 1 Kleinbuchstabe</li><li>min. 1 Ziffer</li><li>min. 1 Sonderzeichen</li></ul>',
        InvalidCredentials: 'Das aktuelle Passwort ist falsch.',
        NewPasswordEqualsCurrent: 'Das neue Passwort darf nicht dem bisherigen Passwort entsprechen.',
        UnexpectedError: 'Bei der Bearbeitung der Anfrage ist ein Fehler aufgetreten.'
      },
      SuccessToast: {
        Title: 'Passwort geändert',
        Message: 'Ihr Password wurde erfolgreich geändert. Sie müssen sich nicht neu anmelden.'
      },
      Back: 'Abbrechen',
      Submit: 'Speichern'
    },
    ChangeUserInfo: {
      Title: 'Benutzerprofil',
      UserName: 'Nutzername:',
      EMail: 'E-Mail Adresse:',
      ChangeEmailNotice:
        'Wenn Sie eine andere E-Mail Adresse angeben, müssen sie die neue E-Mail Adresse bestätigen, bevor die Änderung übernommen wird.',
      RequestFailed: 'Bei der Bearbeitung der Anfrage ist ein Fehler aufgetreten.',
      SuccessToast: {
        Title: 'Nutzerdaten aktualisiert',
        Message: 'Ihr Benutzerinformationen wurden gespeichert.'
      },
      EmailVerificationPendingToast: {
        Title: 'E-Mail muss bestätigt werden',
        Message:
          'Ihr Benutzerinformationen wurden gespeichert. Klicken Sie auf den Link in der Mail, welche wir Ihnen geschickt haben, um die neue E-Mail zu bestätigen.'
      },
      Back: 'Abbrechen',
      Submit: 'Speichern'
    },
    Login: {
      Title: 'Anmelden',
      EMail: 'E-Mail Adresse:',
      Password: 'Passwort:',
      CookieNotice: 'Wenn Sie sich anmelden, wird ihre aktive Sitzung in einem Cookie gespeichert.',
      Submit: 'Anmelden',
      LoginFailed: 'Die Anmeldung ist fehlgeschlagen. Prüfen Sie Ihre E-Mailadresse und Ihr Passwort.'
    }
  },
  Portal: {
    General: {
      Back: 'Zurück',
      Cancel: 'Abbrechen',
      Save: 'Speichern',
      Done: 'Fertig',
      BackToLandingPage: 'Startseite',
      IllustrationAlt: 'Eine Illustration, welche "{{description}}" symbolisiert.',
      CopyToClipboard: 'In die Zwischenablage kopieren',
      OpenInNewTab: 'In neuem Tab öffnen',
      CharacterLimit: '{{current}} von {{maximum}} Zeichen',
      ApplyChanges: 'Änderungen übernehmen'
    },
    UserInfoPopover: {
      Text: 'Sie sind angemeldet als:\n<strong>{{userName}}</strong>',
      Administration: 'Administration',
      EditUserInfo: 'Benutzerinformation',
      ChangePassword: 'Passwort ändern',
      Logout: 'Abmelden'
    },
    LandingPage: {
      Title: 'Startseite',
      Pages: {
        Organizations: 'Organisationen'
      },
      Badges: {
        OrganizationCount: 'Organisationen'
      },
      AdministratorWarning:
        'Sie sind mit einem Administrator-Konto angemeldet und haben unbeschränkten Zugriff auf alle Organisationen - auch die von anderen Benutzern.',
      NoOrganizations:
        'Sie sind keinen Organisationen zugehörig.\nErstellen Sie eine neue Organisation, um Turniere anzulegen und zu bearbeiten',
      NewOrganization: 'Neue Organisation',
      OrganizationTile: {
        Open: 'öffnen'
      }
    },
    Administration: {
      Title: 'Administration',
      Pages: {
        Users: 'Benutzer'
      },
      Badges: {
        UserCount: 'Benutzer'
      },
      NewUser: 'Neuer Benutzer',
      Users: {
        TableLabel: 'Benutzer in dieser turnierplan.NET-Instanz',
        Id: 'ID',
        Name: 'Name',
        EMail: 'E-Mail',
        CreatedAt: 'Erstellt am',
        LastPasswordChange: 'Letzte Passwortänderung',
        Administrator: 'Admin'
      },
      DeleteUser: {
        Title: 'Benutzer löschen',
        Info: 'Wenn Sie eine Benutzer löschen, werden automatisch alle darin enthaltenen Organisationen, Turniere, Spielstätten sämtliche hochgeladenen Bilder mitgelöscht. Diese Aktion kann nicht widerrufen werden!',
        IdConfirmation: 'Benutzer-ID:',
        SuccessToast: {
          Title: 'Benutzer wurde gelöscht',
          Message: 'Der Benutzer wurde gelöscht.'
        }
      }
    },
    CreateUser: {
      Title: 'Neuen Benutzer',
      LongTitle: 'Neuen Benutzer erstellen',
      Form: {
        UserName: 'Name',
        UserNameInvalid: 'Der Name eines neuen Nutzers muss zwischen 1 und 100 Zeichen lang sein.',
        Email: 'E-Mailadresse',
        EmailInvalid: 'Die E-Mailadresse ist ungültig oder zu lang (max. 100 Zeichen)',
        Password: 'Passwort',
        PasswordInvalid: 'Das Passwort ist ungültig oder zu lang (max. 64 Zeichen)'
      },
      UserNotice: 'Der erstellte Nutzer kann sich unmittelbar danach mit E-Mail und Passwort anmelden.',
      Submit: 'Erstellen'
    },
    CreateOrganization: {
      Title: 'Neue Organisation',
      LongTitle: 'Neue Organisation erstellen',
      Form: {
        Name: 'Name',
        NameInvalid: 'Der Name einer neuen Organisation muss zwischen 1 und 40 Zeichen lang sein.',
        NameValid: 'Dieser Name kann verwendet werden.'
      },
      UserNotice: 'Eine Organisation ist z.B. Ihr Sportverein oder Ihre Firma.',
      Submit: 'Erstellen'
    },
    ViewOrganization: {
      Pages: {
        Tournaments: 'Turniere',
        Venues: 'Spielstätten',
        ApiKeys: 'API-Schlüssel',
        Settings: 'Einstellungen'
      },
      Badges: {
        TournamentCount: 'Turniere',
        VenueCount: 'Spielstätten',
        ApiKeyCount: 'API-Schlüssel'
      },
      NewTournament: 'Neues Turnier',
      NewVenue: 'Neue Spielstätte',
      NewApiKey: 'Neuer API-Schlüssel',
      NoTournaments: 'In dieser Organisation gibt es aktuell keine Turniere.\nErstellen Sie ein Turner mit der Schaltfläche oben rechts.',
      NoVenues:
        'In dieser Organisation gibt es aktuell keine Spielstätten.\nErstellen Sie eine Spielstätte mit der Schaltfläche oben rechts.',
      TournamentExplorer: {
        EmptyFolder: 'In diesem Ordner befinden sich keine Turniere. Wählen Sie links einen anderen Ordner oder erstellen Sie ein Turnier.',
        RenameFolder: {
          Button: 'Name ändern',
          Title: 'Ordner umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für den Ordner ein:',
          RequiredFeedback: 'Der Ordername darf nicht leer sein.',
          MaxLengthFeedback: 'Der Ordername darf max. {{maxLength}} Zeichen lang sein.'
        },
        Timetable: 'Zeitplan',
        Statistics: 'Statistik',
        Tile: {
          Open: 'öffnen',
          Visibility: {
            Private: 'Dieses Turnier ist privat.',
            Public: 'Dieses Turnier ist öffentlich sichtbar.'
          }
        }
      },
      VenueTile: {
        NoDescription: 'Keine Beschreibung vorhanden',
        Open: 'öffnen'
      },
      ApiKeys: {
        TableLabel: 'API Schlüssel dieser Organisation',
        Id: 'ID',
        Description: 'Bezeichnung',
        CreatedAt: 'Erstellt am',
        ExpiryDate: 'Ablaufdatum',
        IsActive: 'Aktiv',
        Expired: 'Dieser API-Schlüssel ist abgelaufen',
        NoApiKeys: 'Keine API-Schlüssel vorhanden',
        ViewCharts: 'Aufrufstatistik',
        DeleteToast: {
          Title: 'API-Schlüssel wurde gelöscht',
          Message: 'Der API-Schlüssel wurde gelöscht und kann nun nicht mehr für Anfragen verwendet werden.'
        }
      },
      RbacWidget: {
        Info: 'Verwalten Sie, welche Nutzer auf diese Organisation zugreifen können und welche Aktionen sie durchführen können.'
      },
      DeleteWidget: {
        Title: 'Organisation löschen',
        Info: 'Wenn Sie eine Organisation löschen, werden automatisch alle darin enthaltenen Turniere, Spielstätten sämtliche hochgeladenen Bilder mitgelöscht. Diese Aktion kann nicht widerrufen werden!',
        SuccessToast: {
          Title: 'Organisation wurde gelöscht',
          Message: 'Ihre Organisation wurde gelöscht.'
        }
      },
      Settings: {
        Rename: {
          Button: 'Umbenennen',
          Title: 'Organisation umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für die Organisation ein:',
          MaxLengthFeedback: 'Der Name der Organisation darf max. {{maxLength}} Zeichen lang sein.'
        }
      }
    },
    CreateTournament: {
      Title: 'Neues Turnier erstellen',
      Form: {
        Name: 'Name',
        NameInvalid: 'Der Name eines neuen Turniers muss zwischen 1 und 60 Zeichen lang sein.',
        NameValid: 'Dieser Name kann verwendet werden.',
        Folder: 'Ordner',
        FolderTooltip: 'Mithilfe von Ordnern können Sie Ihre Turniere organisieren.',
        UseNoFolder: 'Keinen Ordner verwenden',
        UseExistingFolder: 'Zu bestehendem Ordner hinzufügen',
        ExistingFolderDisabledTooltip:
          'In Ihrer aktuellen Organisation gibt es aktuell keine Ordner. Erstellen Sie mit der unteren Option einen neuen Ordner.',
        CreateNewFolder: 'Neuen Ordner erstellen',
        NamePlaceholder: 'Geben Sie den Namen für den neuen Ordner ein...',
        FolderNameInvalid: 'Der Name eines neuen Ordners muss zwischen 1 und 60 Zeichen lang sein.',
        FolderNameValid: 'Dieser Name kann verwendet werden.',
        Visibility: 'Sichtbarkeit',
        VisibilityTooltip: 'Legen Sie fest, ob das Turnier von jedem gesehen kann oder nur, wenn Sie eingeloggt sind.'
      },
      OrganizationNotice: 'Es wird ein neues Turnier in der Organisation <span class="fw-bold">{{organizationName}}</span> angelegt.',
      VisibilityPrivateNotice:
        'Ihr Turnier wird auf <em>Privat</em> gestellt. Wenn Sie einen PDF-Spielplan mit QR-Code erstellen, kann der QR-Code nicht von Ihren Gästen verwendet werden.',
      Submit: 'Erstellen'
    },
    ViewTournament: {
      Pages: {
        MatchPlan: 'Spielplan',
        Groups: 'Gruppen',
        Teams: 'Mannschaften',
        Ranking: 'Platzierung',
        Documents: 'PDF',
        Share: 'Teilen',
        Settings: 'Einstellungen'
      },
      Badges: {
        Date: 'Spieltag',
        MatchCount: 'Spiele',
        GoalCount: 'Tore',
        GroupCount: 'Gruppen',
        TeamCount: 'Mannschaften',
        RankingsCount: 'Platzierungen',
        DocumentsCount: 'Dokumente'
      },
      MatchPlan: {
        TableLabel: 'Spielplan des Turniers',
        NoMatches: 'Keine Spiele vorhanden',
        AccumulateGoals: 'Tore zählen',
        CreateMatchPlan: 'Spielplan erstellen',
        Index: 'Nr.',
        Group: 'Gruppe',
        Court: 'Platz',
        Kickoff: 'Anstoß',
        Teams: 'Spielpaarung',
        Outcome: 'Ergebnis',
        ShowMatchTree: 'zum Turnierbaum',
        ShowMatchList: 'zum Spielplan'
      },
      Group: {
        NoGroups: 'Keine Gruppen vorhanden',
        NoTeams: 'Keine Mannschaften in dieser Gruppe',
        Position: 'Pos.',
        Team: 'Mannschaft',
        Matches: 'Sp.',
        MatchesWon: 'G',
        MatchesDrawn: 'U',
        MatchesLost: 'V',
        Score: 'Tore',
        ScoreDifference: 'TD',
        Points: 'Pkt.',
        Rename: {
          Button: 'Gruppennamen ändern',
          Title: 'Gruppennamen ändern',
          EnterNewName: 'Geben Sie den neuen Namen für die Gruppe ein:',
          MaxLengthFeedback: 'Der Gruppenname darf max. {{maxLength}} Zeichen lang sein.',
          EmptyAllowed: 'Sie können das Feld leerlassen, um zum Standardnamen ("Gruppe A-Z") zurückzukehren.'
        }
      },
      Teams: {
        TableLabel: 'Mannschaften des Turniers',
        NoTeams: 'Keine Mannschaften vorhanden',
        Name: 'Name',
        Priority: {
          Header: 'Priorität',
          Tooltip: 'Bei Gleichstand der Gruppenergebnisse wird die Mannschaft mit der höheren Priorität höher platziert.'
        },
        OutOfCompetition: {
          Header: 'außer Konkurrenz',
          Tooltip: 'Eine Mannschaft, die außer Konkurrenz (a.K.) spielt, wird in ihrer Gruppe immer auf dem letzten Platz platziert.'
        },
        EntryFeePaid: {
          Header: 'Startgebühr',
          Tooltip:
            'Vermerken Sie, ob die jeweilige Mannschaft die Startgebühr für das Turnier bezahlt hat. Dies hat keinen Einfluss auf die Berechnung.',
          PaidAt: 'bezahlt am:',
          NotPaid: 'nicht bezahlt',
          Pay: 'Bezahlen',
          ResetConfirm: 'Zurücksetzen auf\n"nicht bezahlt"?'
        },
        Rename: {
          Button: 'Name ändern',
          Title: 'Mannschaft umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für die Mannschaft ein:',
          RequiredFeedback: 'Der Mannschaftsname darf nicht leer sein.',
          MaxLengthFeedback: 'Der Mannschaftsname darf max. {{maxLength}} Zeichen lang sein.'
        }
      },
      Ranking: {
        TableLabel: 'Endplatzierungen des Turniers',
        NoRankings: 'Keine Platzierungen vorhanden',
        Position: 'Position',
        Team: 'Mannschaft'
      },
      Documents: {
        Name: 'Name',
        LastModifiedAt: 'Zuletzt bearbeitet',
        Generations: 'Abrufe',
        Configure: 'Einstellungen',
        Download: 'Herunterladen',
        Delete: 'Löschen',
        Create: 'Neues Dokument',
        Copy: 'Dokument kopieren',
        NoDocuments: 'Es sind aktuell keine Dokumente vorhanden.',
        OpenPreview: 'Vorschau',
        Types: {
          MatchPlan: 'Spielplan',
          RefereeCards: 'Schiri-Karten',
          Receipts: 'Quittungen'
        },
        Rename: {
          Title: 'Dokument umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für das Dokument ein:',
          MaxLengthFeedback: 'Der Dokument-Name darf max. {{maxLength}} Zeichen lang sein.'
        },
        DeleteToast: {
          Title: 'Dokument wurde gelöscht',
          Message: 'Das Dokument wurde gelöscht.'
        },
        ConfigureModal: {
          Title: 'Dokument bearbeiten',
          ValidationErrorsToast: {
            Title: 'Eingabefehler',
            Message: 'Beheben Sie alle Eingabefehler, bevor die Konfiguration gespeichert werden kann.'
          },
          SuccessToast: {
            Title: 'Dokument aktualisiert',
            Message: 'Die Dokumentkonfiguration wurde erfolgreich aktualisiert.'
          },
          UpdateFailedToast: {
            Title: 'Fehler',
            Message: 'Beim Speichern des Dokumentes ist ein Fehler aufgetreten. Prüfen Sie die Konfiguration und versuchen Sie es erneut.'
          },
          Receipts: {
            Amount: 'Betrag',
            AmountInvalid: 'Der Betrag muss min 0,01 betragen.',
            Currency: 'Währung',
            CurrencyInvalid: 'Die Währung muss angegeben werden und darf max. 10 Zeichen lang sein.',
            HeaderInfo: 'Zusatz in der Kopfzeile (optional)',
            HeaderInfoTooltip: 'Sofern vorhanden, wird dieser Text zusätzlich zum Namen des Turniers angegeben.',
            HeaderInfoInvalid: 'Der angegebene Text darf max. 100 Zeichen lang sein.',
            SignatureLocation: 'Ort der Unterschrift (optional)',
            SignatureLocationInvalid: 'Der angegebene Ort darf max. 100 Zeichen lang sein.',
            SignatureRecipient: 'Name des Empfängers (optional)',
            SignatureRecipientTooltip:
              'Wenn angegeben, wird dieser Zahlungsempfänger im Bereich der Unterschrift angegeben. Als Standard steht dort "(Turnierleitung)".',
            SignatureRecipientInvalid: 'Der angegebene Text darf max. 100 Zeichen lang sein.',
            ShowSponsorLogo: 'Sponsorlogo abbilden (falls vorhanden)',
            CombineSimilarTeams: 'Mannschaften mit ähnlichem Namen kombinieren',
            CombineSimilarTeamsTooltip:
              'Mannschaften, welche sich nur durch eine bis zu dreistellige Zahl am Ende des Namens unterscheiden, werden auf eine Quittung gedruckt. (römische Zahlen bis 10 werden unterstützt)',
            ReducedAmountSection: 'Reduzierte Startgebühr',
            ReducedAmountTooltip:
              'Wenn Mannschaften kombiniert werden können Sie hier festlegen, dass ab einer gewissen Anzahl an Mannschaften, die Startgebühr pro Mannschaft sinkt.',
            ReducedAmount: 'Betrag ab {{teamCount}} Mannschaften',
            NoReducedAmounts: 'Keine Einträge vorhanden. Alle Mannschaften bezahlen die gleiche Startgebühr.',
            AddReducedAmount: 'Neuer Eintrag',
            RemoveReducedAmount: 'Eintrag entfernen'
          },
          MatchPlan: {
            OrganizerNameOverride: 'Veranstalter (optional)',
            OrganizerNameOverrideTooltip:
              'Standardmäßig wird auf dem Spielplan der Name der Organisation abgebildet. Wenn dieses Feld ausgefüllt wird, wird stattdessen der angegebene Text abgedruckt.',
            OrganizerNameOverrideInvalid: 'Der Text muss entweder leer sein oder darf max. 100 Zeichen lang sein.',
            TournamentNameOverride: 'Turniername (optional)',
            TournamentNameOverrideTooltip:
              'Standardmäßig wird auf dem Spielplan der Name des Turniers abgebildet. Wenn dieses Feld ausgefüllt wird, wird stattdessen der angegebene Text abgedruckt.',
            TournamentNameOverrideInvalid: 'Der Text muss entweder leer sein oder darf max. 100 Zeichen lang sein.',
            VenueOverride: 'Spielort (optional)',
            VenueOverrideTooltip:
              'Standardmäßig wird auf dem Spielplan (sofern vorhanden) der Name der verknüpften Spielstätte abgebildet. Wenn dieses Feld ausgefüllt wird, wird stattdessen der angegebene Text abgedruckt.',
            VenueOverrideInvalid: 'Der Text muss entweder leer sein oder darf max. 100 Zeichen lang sein.',
            DateFormat: 'Angabe vom Datum',
            DateFormats: {
              NoDate: 'nicht angeben',
              DateOnly: 'nur Datum angeben (z.B. 31.07.2024)',
              DateAndDayOfWeek: 'Datum und Wochentag angeben (z.B. Mittwoch, den 31.07.2024)',
              DateAndDayOfWeekAndTimeOfDay: 'Datum, Wochentag und Tageszeit angeben (z.B. Mittwochnachmittag, den 31.07.2024)'
            },
            OutcomesType: 'Ergebnisse',
            OutcomesTypes: {
              HideOutcomeStructures: 'Keine Spalten & Tabellen für Ergebnisse anzeigen',
              ShowEmptyOutcomeStructures: 'Spalten & Tabellen anzeigen',
              ShowOutcomeStructuresWithOutcomes: 'Spalten & Tabellen anzeigen und ausfüllen'
            },
            IncludeRankingTable: 'Tabelle für Endplatzierungen anzeigen',
            IncludeRankingTableTooltip:
              'Diese Option ist nicht verfügbar, wenn oben die Option "Keine Spalten & Tabellen für Ergebnisse anzeigen" selektiert ist.',
            IncludeQrCode: 'QR-Code und Web-Link anzeigen',
            IncludeQrCodeNotice:
              'Der QR-Code kann nur verwendet werden, wenn das Turnier unter dem Reiter <strong>Teilen</strong> auf <strong>Öffentlich</strong> gestellt ist.',
            IncludeQrCodeTooltip:
              'Bildet einen QR-Code ab, den Ihre Besucher scannen können um den Online-Spielplan auf ihrem Mobilgerät einsehen zu können.'
          }
        }
      },
      Share: {
        Title: 'Turnier teilen',
        Info: 'Wenn das Turnier auf <em>Öffentlich</em> gestellt ist, können Sie einen Link kopieren und versenden, mit dem jeder den Spielplan und die Ergebnisse sehen kann.',
        QrCodeNotice:
          'Wenn Sie einen PDF-Spielplan mit QR-Code erstellen, muss das Turnier auf Öffentlich gesetzt sein, damit der QR-Code von Ihren Gästen gescannt werden kann.',
        QrCodeDownloadFileName: 'QR-Code Spielplan {{tournamentName}}',
        VisitorLink: {
          Title: 'Besucher-Link',
          Link: 'Der folgende Link ist für Ihre Besucher empfohlen. Sie können aktuelle Spielstände & Ergebnisse sowie Informationen zur Spielstätte sehen. Die Seite ist für die Betrachtung auf Laptop oder Smartphone optimiert.',
          QrCode: 'Folgender QR-Code beinhaltet ebenfalls diesen Link:',
          QrCodeSave: 'QR-Code als Bild speichern',
          ViewsCounter: 'Aufrufe:'
        },
        FullscreenLink: {
          Title: 'Anzeigetafel',
          Link: 'Der folgende Link ist empfohlen, wenn Sie für Ihre Besucher eine Anzeigetafel mit aktuellen Ergebnissen präsentieren. Die Seite ist für die Betrachtung im Vollbild-Modus optimiert und aktualisiert automatisch in regelmäßigen Abständen.'
        },
        Settings: 'Einstellungen',
        SettingsInfo: 'Konfigurieren Sie, wie die Turniere öffentlich präsentiert werden.',
        PresentationConfig: {
          CopyFromOtherTournament: 'von anderem Turnier übernehmen',
          Header1: 'Titelzeile 1:',
          Header2: 'Titelzeile 2:',
          HeaderCustomValuePlaceholder: 'Eigenen Text eingeben',
          HeaderCustomValueInvalid: 'Der Text muss zwischen 1 und 60 Zeichen lang sein.',
          HeaderLineContent: {
            TournamentName: 'Name des Turniers',
            OrganizerName: 'Name der Organisation',
            CustomValue: 'Eigener Text'
          },
          ShowResults: 'Ergebnisse anzeigen:',
          ResultsMode: {
            Default: 'Alle Ergebnisse anzeigen',
            OnlyMatchOutcomes: 'Nur Spielergebnisse, keine Gruppenergebnisse',
            NoResults: 'Gar keine Ergebnisse anzeigen'
          },
          ShowOrganizerLogo: 'Veranstalter-Logo darstellen',
          ShowSponsorLogo: 'Sponsoren-Logo darstellen',
          SuccessToast: {
            Title: 'Einstellungen gespeichert',
            Message: 'Ihre Änderungen an der Konfiguration wurden erfolgreich gespeichert'
          }
        }
      },
      Settings: {
        Rename: {
          Button: 'Umbenennen',
          Title: 'Turnier umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für das Turnier ein:',
          MaxLengthFeedback: 'Der Turnier-Name darf max. {{maxLength}} Zeichen lang sein.'
        },
        EditMatchPlan: {
          Title: 'Spielplan bearbeiten',
          EditAndRecreateInfo:
            'Unter <span class="text-decoration-underline">Spielplan bearbeiten</span> können Sie individuelle Spiele verschieben oder die Teilnehmer anpassen. Bei <span class="text-decoration-underline">Turnier neu erstellen</span> können Sie einen völlig neuen Spielplan erzeugen.',
          ButtonEdit: 'Spielplan bearbeiten',
          ButtonCreate: 'Spielplan erstellen',
          ButtonRecreate: 'Turnier neu erstellen'
        },
        EditVenue: {
          Title: 'Spielstätte',
          Info: 'Wählen Sie die Spielstätte, welche den Besuchern der öffentlichen Webseite angezeigt wird.',
          CurrentVenue: 'Spielstätte:',
          Open: 'Details',
          NoVenue: 'Es ist keine Spielstätte festgelegt.',
          Button: 'Ändern'
        },
        EditComputationConfig: {
          Title: 'Berechnungsmodus',
          Info: 'Für dieses Turnier wird folgende Berechnungsformel verwendet:',
          Button: 'Anpassen',
          Properties: {
            MatchWonPoints: 'Punkte für Sieg:',
            MatchDrawnPoints: 'Punkte für Unentschieden:',
            MatchLostPoints: 'Punkte für Niederlage:',
            ComparisonModes: {
              Label: 'Platzierungsregel in Gruppen:',
              Values: {
                ByPoints: 'Punkte',
                ByScoreDifference: 'Tordifferenz',
                ByScore: 'geschossene Tore',
                ByDirectComparison: 'direkter Vergleich'
              }
            }
          }
        },
        EditImages: {
          Title: 'Logos & Bilddateien',
          ImageAlt: {
            OrganizerLogo: 'Veranstalter-Logo',
            SponsorLogo: 'Sponsor-Logo',
            SponsorBanner: 'Sponsor-Banner'
          },
          Change: 'Ändern',
          Chooser: {
            Title: 'Bild hochladen oder auswählen',
            Remove: 'Bild entfernen',
            NoImages: 'Laden Sie Ihr erstes Bild hoch...',
            Upload: 'Hochladen',
            UploadFailed: 'Das Bild konnte nicht hochgeladen werden. Prüfen Sie die Maße und die maximale Dateigröße.',
            Constraints: {
              SquareLargeLogo:
                'Das Bild muss quadratisch sein mit einer Auflösung zwischen 50x50 und 3000x3000 Pixel. Die maximale Dateigröße beträgt 1 MB.',
              SponsorBanner:
                'Das Bild muss mindestens 50px hoch sein und darf maximal 3000px breit sein. Das Seitenverhältnis muss zwischen 3:1 und 5:1 liegen. Die maximale Dateigröße beträgt 1 MB.'
            },
            DetailView: {
              Title: 'Hier sehen Sie die Detailinformationen zu folgendem Bild:',
              Name: 'Dateiname: {{value}}',
              CreatedAt: 'Hochgeladen am: {{value}}',
              FileSize: 'Dateigröße: {{value}} KB',
              Resolution: 'Auflösung: {{width}}x{{height}} px'
            }
          }
        },
        MoveToAnotherFolder: {
          Title: 'Turnier verschieben',
          CurrentFolderName: 'Dieses Turnier liegt aktuell in folgendem Ordner:',
          CurrentlyNoFolder: 'Dieses Turnier liegt aktuell nicht in einem Ordner ab.',
          Button: 'Verschieben',
          Info: 'In welchen Ordner soll das Turnier verschoben werden?',
          UseNoFolder: 'Keinen Ordner verwenden',
          UseExistingFolder: 'Zu bestehendem Ordner hinzufügen',
          ExistingFolderDisabledTooltip:
            'In Ihrer aktuellen Organisation gibt es aktuell keine weiteren Ordner. Erstellen Sie mit der unteren Option einen neuen Ordner.',
          CreateNewFolder: 'Neuen Ordner erstellen',
          NamePlaceholder: 'Geben Sie den Namen für den neuen Ordner ein...',
          FolderNameInvalid: 'Der Name eines neuen Ordners muss zwischen 1 und 60 Zeichen lang sein.',
          FolderNameValid: 'Dieser Name kann verwendet werden.'
        }
      },
      AddDocumentDialog: {
        Title: 'Neues Dokument erstellen',
        Info: 'Wählen Sie den gewünschten Dokumenttyp. Anschließend können Sie Inhalt und Aussehen des Dokuments konfigurieren sowie die PDF-Datei herunterladen.'
      },
      CopyDocumentDialog: {
        Title: 'Dokument kopieren',
        Info: 'Kopieren Sie ein bestehendes Dokument von einem anderen Turnier. Sämtliche Einstellungen des Dokuments werden hierbei übernommen.',
        SelectFolder: 'Wählen Sie einen Ordner, welcher ein Turnier beinhaltet.',
        SelectTournament: 'Wählen Sie einen Turnier, welches ein Dokument beinhaltet.'
      },
      EditMatchDialog: {
        Title: 'Spielergebnis melden',
        Index: 'Nr.',
        Kickoff: 'Anstoß',
        Type: 'Spieltyp',
        Group: 'Gruppe',
        OutcomeType: {
          Standard: 'Standard',
          AfterOvertime: 'nach Verlängerung (n.V.)',
          AfterPenalties: 'nach Elfmeterschießen (n.E.)',
          SpecialScoring: 'Sonderwertung (§)'
        },
        Clear: 'Ergebnis entfernen',
        SaveLiveSuffix: 'speichern',
        SaveFinished: 'Endergebnis speichern',
        OverwriteNotice:
          'Für dieses Spiel wurde bereits ein Ergebnis gemeldet. Wenn das Ergebnis verändert oder entfernt wird, kann dies Auswirkungen auf den gesamten restlichen Verlauf des Turniers haben.',
        DrawNotAllowedNotice: 'Ein Entscheidungsspiel darf nicht in einem Unentschieden enden. Bitte passen Sie das Ergebnis an.'
      },
      EditMatchToast: {
        Title: 'Änderungen übernommen',
        MessageClear: 'Das bestehende Ergebnis wurde entfernt.',
        MessageSave: 'Das Spielergebnis wurde gespeichert.'
      },
      ComputationConfigurationDialog: {
        StandardComparisonModes: 'Häufig verwendet:',
        NonStandardComparisonModes: 'Alle Platzierungsregeln:'
      },
      AssignVenueDialog: {
        Title: 'Spielstätte festlegen',
        Info: 'Weisen Sie dem Turnier die korrekte Spielstätte zu oder entfernen Sie eine bisherige Zuweisung.',
        NoVenues: 'In der aktuellen Organisation gibt es aktuell keine Spielstätten.',
        NoVenueOption: 'Keine Spielstätte verknüpfen'
      },
      RbacWidget: {
        Info: 'Verwalten Sie, welche Nutzer auf dieses Turnier zugreifen können und welche Aktionen sie durchführen können.'
      },
      DeleteWidget: {
        Title: 'Turnier löschen',
        Info: 'Wenn Sie ein Turnier löschen, werden alle Mannschaften, Gruppen, Spiele und Ergebnisse gelöscht. Diese Aktion kann nicht widerrufen werden!',
        SuccessToast: {
          Title: 'Turnier wurde gelöscht',
          Message: 'Ihr Turnier wurde gelöscht.'
        }
      }
    },
    ConfigureTournament: {
      Title: 'Turnierplanung \u00b7 {{tournament}}',
      Subtitle: 'Konfigurieren Sie die Mannschaften, Gruppen und den Spielplan für das Turnier.',
      DestructiveWarning: {
        Alert: 'Achtung:',
        Text: 'In diesem Turnier sind bereits Spielergebnisse hinterlegt. Wenn Sie das Turnier bearbeiten, werden die bestehenden Spielergebnisse gelöscht!',
        CheckboxLabel: 'Ich habe den Hinweis gelesen und möchte den Spielplan neu erstellen'
      },
      Sections: {
        General: {
          Title: 'Allgemein',
          KickoffDateTime: 'Wann beginnt das Turnier?',
          KickoffDateTimeInPast: 'Der angegebene Turnierbeginn liegt an einem Tag in der Vergangenheit.'
        },
        Participants: {
          Title: 'Teilnehmende Mannschaften',
          GroupWithName: 'Gruppe {{alphabeticalId}}: {{displayName}}',
          GroupWithoutName: 'Gruppe {{alphabeticalId}}',
          DragDropPlaceholder: 'hier einfügen...',
          TeamEntry: '{{index}}. {{name}}',
          NoTeamsNotice: 'Keine Mannschaften vorhanden',
          NewTeamNamePlaceholder: 'Neue Mannschaft',
          AddTeamButton: 'Hinzufügen',
          NoGroupsNotice: 'Es existieren aktuell keine Gruppen',
          AddGroupButton: 'Neue Gruppe hinzufügen',
          ShuffleGroupsButton: 'Gruppen würfeln',
          GroupLimitReached: 'Die maximale Gruppenanzahl (26) ist erreicht.'
        },
        GroupPhase: {
          Title: 'Gruppenphase',
          OrderLabel: 'In welcher Reihenfolge finden die Gruppenspiele statt?',
          OrderSequential: 'AABB AABB',
          OrderAlternating: 'ABAB ABAB',
          NumberOfRoundsLabel: 'Wie viele Runden werden gespielt?',
          InvalidNumberOfRounds: 'Die Anzahl muss zwischen 1 und 4 liegen.',
          NumberOfCourtsLabel: 'Wie viele Spielfelder gibt es?',
          InvalidNumberOfCourts: 'Die Anzahl Spielfelder muss zwischen 1 und 10 liegen.',
          PlayTimeLabel: 'Dauer der Spiele:',
          PauseTimeLabel: 'Pausen zwischen den Spielen:'
        },
        FinalsPhase: {
          Title: 'Finalrunde',
          NoTemplates: 'Für die aktuelle Gruppenkonfiguration gibt es keine Vorlagen für eine Finalrunde.',
          EnableCheckbox: 'Finalrunde spielen',
          IntermediatePause: 'Pause zwischen Gruppen- und Finalrunde:',
          FirstRoundLabel: 'Mit welcher Finalrunde soll begonnen werden?',
          RankingMatchesLabel: 'Platzierungsspiele:',
          ThirdPlacePlayoff: 'Spiel um Platz 3',
          ThirdPlacePlayoffTooltip: 'Dieses Spiel um Platz 3 wird mit den beiden Verlierern der Halbfinalspiele gespielt.',
          AdditionalPlayoffs: 'Weitere Platzierungsspiele',
          AdditionalPlayoffsTooltip: 'Die Begegnungen für diese Platzierungsspiele können selber festgelegt werden.',
          NumberOfCourtsLabel: 'Wie viele Spielfelder gibt es?',
          InvalidNumberOfCourts: 'Die Anzahl Spielfelder muss zwischen 1 und 10 liegen.',
          FinalsRound: {
            Final: 'Finale',
            SemiFinal: 'Halbfinale',
            QuarterFinal: 'Viertelfinale',
            EighthFinal: 'Achtelfinale'
          },
          PlayTimeLabel: 'Dauer der Spiele:',
          PauseTimeLabel: 'Pausen zwischen den Spielen:'
        },
        AdditionalPlayoffs: {
          Title: 'Zusätzliche Platzierungsspiele',
          InfoText: 'Die folgenden Platzierungsspiele können konfiguriert werden:',
          MatchLabel: 'Spiel um Platz {{position}}',
          DuplicateTeam: 'Dieselbe Mannschaft ist zweimal selektiert.',
          DropdownPlaceholder: 'Mannschaft selektieren...'
        }
      },
      ConfigurationError: 'Konfigurationsfehler',
      SaveNotice: 'Die Änderungen werden erst beim Speichern wirksam.',
      ValidationErrorsNotice: 'Die Konfiguration kann derzeit nicht übernommen werden, da es Fehler gibt.',
      AcceptWarningNotice: 'Bitte akzeptieren Sie den obigen Warnhinweis.',
      SuccessToast: {
        Title: 'Turnier wurde aktualisiert',
        Message: 'Die Änderungen am Turnier wurden erfolgreich durchgeführt und gespeichert.'
      },
      ValidationErrors: {
        Title: 'Fehler beim Speichern',
        Info: 'Beim Speichern der Änderungen am Turnier ist einer oder mehrere Fehler aufgetreten:'
      },
      Submit: 'Übernehmen'
    },
    EditMatchPlan: {
      Title: 'Spielplan bearbeiten \u00b7 {{tournament}}',
      Subtitle: 'Bearbeiten und verschieben Sie individuelle Spiele.',
      DestructiveWarning: {
        Alert: 'Achtung:',
        Text: 'In diesem Turnier sind bereits Spielergebnisse hinterlegt. Wenn Sie den Spielplan bearbeiten, kann sich dies u.U. auf bereits bestehende Gruppen- und Endplatzierungen auswirken.',
        CheckboxLabel: 'Ich habe den Hinweis gelesen und möchte den Spielplan bearbeiten'
      },
      Table: {
        Index: 'Nr.',
        Type: 'Typ',
        Court: 'Platz',
        Kickoff: 'Anstoß',
        Teams: 'Spielpaarung'
      },
      EditButtonTooltip: 'Spieldaten bearbeiten',
      EditMatch: {
        Court: 'Spielfeld:',
        Kickoff: 'Anstoß:',
        GroupMatchRestriction: 'Bei Gruppenspielen können nur Mannschaften der jeweiligen Gruppe gewählt werden.',
        TeamSelectorA: 'Mannschaft (Heim):',
        TeamSelectorB: 'Mannschaft (Gast):'
      },
      SaveNotice: 'Die Änderungen werden erst beim Speichern wirksam.',
      AcceptWarningNotice: 'Bitte akzeptieren Sie den obigen Warnhinweis.',
      SuccessToast: {
        Title: 'Spielplan wurde gespeichert',
        Message: 'Die Änderungen am Spielplan wurden erfolgreich gespeichert.'
      },
      Submit: 'Übernehmen'
    },
    CreateVenue: {
      Title: 'Neue Spielstätte erstellen',
      Form: {
        Name: 'Name',
        NameInvalid: 'Der Name einer neuen Spielstätte muss zwischen 1 und 60 Zeichen lang sein.',
        NameValid: 'Dieser Name kann verwendet werden.'
      },
      OrganizationNotice: 'Es wird eine neue Spielstätte in der Organisation <span class="fw-bold">{{organizationName}}</span> angelegt.',
      Submit: 'Erstellen'
    },
    ViewVenue: {
      Pages: {
        Details: 'Informationen',
        Settings: 'Einstellungen'
      },
      Details: {
        PublicInformationAlert:
          'Wenn Sie diese Spielstätte mit einem öffentlich sichtbaren Turnier verknüpfen, sind alle Informationen, welche auf dieser Seite angegeben werden, ebenfalls öffentlich sichtbar.',
        Edit: 'Bearbeiten',
        Name: 'Name',
        Rename: {
          Button: 'Bearbeiten',
          Title: 'Spielstätte umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für die Spielstätte ein:',
          RequiredFeedback: 'Der Name der Spielstätte darf nicht leer sein.',
          MaxLengthFeedback: 'Der Name der Spielstätte darf max. {{maxLength}} Zeichen lang sein.'
        },
        Description: 'Beschreibung',
        NoDescription: 'Keine Beschreibung vorhanden',
        EditDescription: {
          Title: 'Beschreibung bearbeiten',
          EnterNewName: 'Geben Sie die Beschreibung für die Spielstätte ein (kann leer gelassen werden):'
        },
        AddressDetails: 'Adressdetails',
        AddressDetailsTooltip: 'Adressdetails sind z.B. Anschrift, Hausnummer oder Stadt.',
        NoAddressDetails: 'Keine Adressdetails vorhanden',
        EditAddressDetails: {
          Title: 'Adressdetails bearbeiten',
          HelpText: 'Sie können bis zu {{maxCount}} Einträge hinzufügen. Beispiele hierfür sind z.B. Straße, Hausnummer, PLZ oder Ort.',
          InvalidEntry: 'Jeder Eintrag muss zwischen 1 und {{maxLength}} Zeichen lang sein.',
          NoEntries: 'Keine Einträge vorhanden',
          AddEntry: 'Zeile hinzufügen',
          MaximumReached: 'Das Maximum von {{maxCount}} Zeilen ist erreicht'
        },
        ExternalLinks: 'Externe Referenzen',
        ExternalLinksTooltip: 'Externe Links sind z.B. Links zu Kartenanbietern oder einer externen Webseite.',
        NoExternalLinks: 'Keine externen Referenzen vorhanden',
        EditExternalLinks: {
          Title: 'Externe Referenzen bearbeiten',
          HelpText:
            'Sie können bis zu {{maxCount}} Referenzen hinzufügen. Beispiele hierfür sind z.B. Links zu Ihrer Webseite oder einem Kartenanbieter. Bitte beachten Sie, dass externe Links prinzipiell immer als nicht vertrauenswürdig präsentiert werden.',
          InvalidEntry:
            'Jeder Eintrag muss zwischen 1 und {{maxLength}} Zeichen lang sein. Es muss sich auch um einen gültigen HTTPS-Link handeln.',
          NoEntries: 'Keine Links vorhanden',
          AddEntry: 'Link hinzufügen',
          MaximumReached: 'Das Maximum von {{maxCount}} Links ist erreicht'
        },
        UnsavedChanges: 'Sie haben ungespeicherte Änderungen, welche noch übernommen werden müssen.',
        SuccessToast: {
          Title: 'Spielstätte gespeichert',
          Message: 'Ihre Änderungen an dieser Spielstätte wurden gespeichert'
        }
      },
      RbacWidget: {
        Info: 'Verwalten Sie, welche Nutzer auf diese Spielstätte zugreifen können und welche Aktionen sie durchführen können.'
      },
      DeleteWidget: {
        Title: 'Spielstätte löschen',
        Info: 'Wenn Sie eine Spielstätte löschen, wird diese von allen derzeit verknüpften Turnieren entfernt. Diesen Turnieren kann anschließend eine andere Spielstätte zugewiesen werden.',
        SuccessToast: {
          Title: 'Spielstätte wurde gelöscht',
          Message: 'Ihre Spielstätte wurde gelöscht.'
        }
      }
    },
    SelectTournamentDialog: {
      Title: 'Turnier auswählen',
      Info: 'Wählen Sie ein bestehendes Turnier aus der Organisation aus.',
      SelectFolder: 'Wählen Sie einen Ordner, welcher ein Turnier beinhaltet.'
    },
    FolderTimetable: {
      Title: 'Zeitplan \u00b7 {{folder}}',
      Refresh: 'Aktualisieren',
      Break: '{{minutes}} Minute(n) Pause',
      MissingTournaments: 'Das folgende Turnier bzw. die folgenden Turniere können nicht im Zeitplan angezeigt werden:',
      MidnightTournament: 'Dieses Turnier endet nach Mitternacht.',
      OpenInNewTab: 'Turniere in neuem Tab öffnen'
    },
    FolderStatistics: {
      Title: 'Statistik \u00b7 {{folder}}',
      NoStatistics: 'Es sind aktuell keine Statistiken verfügbar.',
      ImportantFacts: 'Die wichtigsten Zahlen auf einen Blick:',
      FinishedMatches: 'Es sind {{done}} von {{total}} Spiele abgeschlossen ({{percent}})',
      MostFrequentOutcomes: 'Die häufigsten Ergebnisse:',
      NumberOfMatches: 'Anzahl Spiele',
      PageViewsPerTournament: 'Seitenaufrufe pro Turnier:',
      NumberOfPageViews: 'Seitenaufrufe',
      StatisticNames: {
        Tournaments: 'Turniere',
        Teams: 'Mannschaften',
        Matches: 'Spiele',
        Groups: 'Gruppen',
        Goals: 'Tore',
        GoalsPerMatch: 'Tore pro Spiel',
        MostSignificantOutcome: 'Deutlichstes Ergebnis',
        MostGoalsPerMatch: 'Meiste Tore pro Spiel',
        NumberOfDecidingMatches: 'Entscheidungsspiele',
        NumberOfPenaltyShootouts: 'Elfmeterschießen',
        NumberOfOverTimes: 'Verlängerungen',
        TotalPublicPageViews: 'Seitenaufrufe insgesamt'
      },
      ExcludedTournamentsNotice: 'Folgende Turniere sind bei dieser Statistik ausgenommen: {{tournamentNames}}'
    },
    CreateApiKey: {
      Title: 'Neuen API-Schlüssel erstellen',
      Form: {
        Name: 'Name',
        NameInvalid: 'Der Name muss zwischen 1 und 25 Zeichen lang sein.',
        NameValid: 'Dieser Name kann verwendet werden.',
        Description: 'Beschreibung',
        DescriptionInvalid: 'Die Beschreibung darf max. 250 Zeichen lang sein.',
        DescriptionValid: 'Diese Beschreibung kann verwendet werden.',
        Validity: 'Gültigkeit',
        Validity1: '1 Tag',
        Validity7: '1 Woche',
        Validity30: '30 Tage',
        Validity90: '90 Tage',
        Validity180: '180 Tage'
      },
      OrganizationNotice: 'Es wird ein neuer API-Schlüssel in der Organisation <span class="fw-bold">{{organizationName}}</span> angelegt.',
      AccessNotice: 'Ein neuer API-Schlüssel erhält standardmäßig Leserechte für die aktuelle Organisation.',
      Submit: 'Erstellen',
      SuccessInformation: {
        Title: 'API-Schlüssel wurde erstellt',
        SecretWarning:
          'Der API-Schlüssel wurde erfolgreich erstellt. Kopieren Sie sich das Kennwort und legen Sie es sicher ab. Wenn Sie diese Seite verlassen, können Sie das Kennwort nicht nachträglich abrufen.',
        Id: 'Schlüssel-ID:',
        Secret: 'Schlüssel-Kennwort:'
      }
    },
    ApiKeyUsage: {
      LoadingData: 'Daten werden geladen',
      TimeRange: 'Zeitraum:',
      Reload: 'Aktualisieren',
      Legend: 'Anfragen',
      NoRequests: 'In diesem Zeitraum gibt es keine Anfragen.',
      TimeRange1: '1 Tag',
      TimeRange7: '7 Tage',
      TimeRange14: '14 Tage',
      TimeRange30: '30 Tage',
      Total: 'Summe:'
    },
    DeleteButton: {
      Cancel: 'Abbrechen',
      ConfirmTooltip: 'Löschvorgang bestätigen'
    },
    DeleteWidget: {
      EnterToConfirm: 'Zur Bestätigung geben Sie bitte &quot;<strong>{{text}}</strong>&quot; in folgendes Textfeld ein:',
      Delete: 'Löschen'
    },
    VisibilitySelector: {
      Private: 'Privat',
      Public: 'Öffentlich'
    },
    ErrorPage: {
      Title: 'Fehler',
      ResourceNotFound: 'Die angeforderte Resource existiert nicht oder Sie\nhaben keine Berechtigung, diese Resource aufzurufen.',
      UnexpectedError:
        'Beim Verarbeiten der Anfrage ist ein unerwarteter Server-Fehler aufgetreten.\nLaden Sie die Seite neu und versuchen Sie es erneut.',
      ErrorDescription: 'Fehlerbeschreibung:'
    },
    RbacManagement: {
      Title: 'Zugriff verwalten',
      ButtonLabel: 'Verwalten',
      Loading: 'Rollenzuweisungen werden geladen',
      NumberOfAssignments: 'Anzahl Rollenzuweisungen:',
      RoleNames: {
        Owner: 'Besitzer',
        Contributor: 'Mitwirkender',
        Reader: 'Leser'
      },
      PrincipalKind: {
        ApiKey: 'API-Schlüssel',
        User: 'Benutzer'
      },
      Id: 'ID:',
      CreatedAt: 'Erstellt am:',
      NotInherited: 'Zuweisung stammt von dieser Resource',
      Inherited: 'Vererbt von:',
      InheritedTooltip: 'Diese Rollenzuweisung existiert implizit aufgrund der Zugehörigkeit zu einer anderen Resource'
    }
  }
};
