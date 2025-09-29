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
          'Das neue Passwort muss folgende Kriterien erfüllen:<ul><li>min. 8 Zeichen lang</li><li>min. 1 Buchstabe</li><li>min. 1 Ziffer</li><li>min. 1 Sonderzeichen</li></ul>',
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
    Login: {
      Title: 'Anmelden',
      UserName: 'Benutzername:',
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
      Change: 'Ändern',
      Delete: 'Löschen',
      Apply: 'Übernehmen',
      BackToLandingPage: 'Startseite',
      IllustrationAlt: 'Eine Illustration, welche "{{description}}" symbolisiert.',
      CopyToClipboard: 'In die Zwischenablage kopieren',
      OpenInNewTab: 'In neuem Tab öffnen',
      UnsavedChanges: 'Sie haben ungespeicherte Änderungen.',
      ApplyChanges: 'Änderungen übernehmen',
      CtrlEnter: 'Strg. + Enter zum Speichern',
      PaginationRange: 'Zeige {{start}} - {{end}} von {{total}}'
    },
    Header: {
      Administrator: 'Administrator',
      UserInfoPopover: {
        Text: 'Sie sind angemeldet als:\n<strong>{{userName}}</strong>',
        Administration: 'Administration',
        ChangePassword: 'Passwort ändern',
        Logout: 'Abmelden'
      }
    },
    LandingPage: {
      Title: 'Startseite',
      Pages: {
        Organizations: 'Organisationen'
      },
      Badges: {
        OrganizationCount: 'Organisationen'
      },
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
        UserName: 'Benutzername',
        FullName: 'Name',
        EMail: 'E-Mail',
        CreatedAt: 'Erstellt am',
        LastPasswordChange: 'Letzte Passwortänderung',
        Administrator: 'Admin'
      },
      EditUser: {
        Title: 'Benutzer bearbeiten',
        Info: 'Ändern Sie die Informationen eines bestehenden Benutzers. Beachten Sie, dass der betroffene Nutzer diese Änderungen unter Umständen nicht direkt sieht.',
        UserName: 'Benutzername:',
        UserNameInvalid: 'Der Benutzername eines Nutzers darf nicht leer sein.',
        FullName: 'Name:',
        Email: 'E-Mailadresse',
        EmailInvalid: 'Die eingegebene E-Mailadresse ist ungültig.',
        IsAdministrator: 'Administrator',
        AdministratorWarning: 'Mit Administratorrechten kann dieser Nutzer ALLES machen!',
        UpdatePassword: 'Passwort ändern',
        Password: 'Neues Passwort:',
        PasswordInvalid: 'Das eingegebene Passwort ist ungültig.',
        SuccessToast: {
          Title: 'Benutzer wurde gespeichert',
          Message: 'Die Benutzerinformationen wurden aktualisiert.'
        }
      },
      DeleteUser: {
        Title: 'Benutzer löschen',
        Info: 'Wenn Sie eine Benutzer löschen, werden die Organisationen des Benutzers nicht mitgelöscht und bleiben weiterhin für alle Administratoren sichtbar.',
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
        UserName: 'Benutzername:',
        UserNameInvalid: 'Der Name eines neuen Nutzers darf nicht leer sein.',
        FullName: 'Name:',
        Email: 'E-Mailadresse:',
        EmailInvalid: 'Die eingegebene E-Mailadresse ist ungültig.',
        Password: 'Passwort:',
        PasswordInvalid: 'Das eingegebene Passwort ist ungültig.'
      },
      UserNotice: 'Der erstellte Nutzer kann sich unmittelbar danach mit Benutzername und Passwort anmelden.',
      Submit: 'Erstellen'
    },
    CreateOrganization: {
      Title: 'Neue Organisation',
      LongTitle: 'Neue Organisation erstellen',
      Form: {
        Name: 'Name',
        NameInvalid: 'Der Name einer neuen Organisation darf nicht leer sein.',
        NameValid: 'Dieser Name kann verwendet werden.'
      },
      UserNotice: 'Eine Organisation ist z.B. Ihr Sportverein oder Ihre Firma.',
      Submit: 'Erstellen'
    },
    ViewOrganization: {
      Pages: {
        Tournaments: 'Turniere',
        Venues: 'Spielstätten',
        PlanningRealms: 'Turnierplaner',
        ApiKeys: 'API-Schlüssel',
        Settings: 'Einstellungen'
      },
      Badges: {
        TournamentCount: 'Turniere',
        VenueCount: 'Spielstätten',
        PlanningRealmCount: 'Turnierplaner',
        ApiKeyCount: 'API-Schlüssel'
      },
      NewTournament: 'Neues Turnier',
      NewVenue: 'Neue Spielstätte',
      NewPlanningRealm: 'Neuer Turnierplaner',
      NewApiKey: 'Neuer API-Schlüssel',
      NoTournaments: 'In dieser Organisation gibt es aktuell keine Turniere.\nErstellen Sie ein Turner mit der Schaltfläche oben rechts.',
      NoVenues:
        'In dieser Organisation gibt es aktuell keine Spielstätten.\nErstellen Sie eine Spielstätte mit der Schaltfläche oben rechts.',
      NoPlanningRealms:
        'In dieser Organisation gibt es aktuell keine Turnierplaner.\nErstellen Sie einen Turnierplaner mit der Schaltfläche oben rechts.',
      OpenPlanningRealm: 'öffnen',
      TournamentExplorer: {
        EmptyFolder: 'In diesem Ordner befinden sich keine Turniere. Wählen Sie einen anderen Ordner oder erstellen Sie ein Turnier.',
        RenameFolder: {
          Button: 'Name ändern',
          Title: 'Ordner umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für den Ordner ein:',
          RequiredFeedback: 'Der Ordername darf nicht leer sein.'
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
          EnterNewName: 'Geben Sie den neuen Namen für die Organisation ein:'
        }
      }
    },
    CreateTournament: {
      Title: 'Neues Turnier erstellen',
      Form: {
        Name: 'Name',
        NameInvalid: 'Der Name eines neuen Turniers darf nicht leer sein.',
        NameValid: 'Dieser Name kann verwendet werden.',
        Folder: 'Ordner',
        FolderTooltip: 'Mithilfe von Ordnern können Sie Ihre Turniere organisieren.',
        UseNoFolder: 'Keinen Ordner verwenden',
        UseExistingFolder: 'Zu bestehendem Ordner hinzufügen',
        ExistingFolderDisabledTooltip:
          'In Ihrer aktuellen Organisation gibt es aktuell keine Ordner. Erstellen Sie mit der unteren Option einen neuen Ordner.',
        CreateNewFolder: 'Neuen Ordner erstellen',
        NamePlaceholder: 'Geben Sie den Namen für den neuen Ordner ein...',
        FolderNameInvalid: 'Der Name eines neuen Ordners darf nicht leer sein.',
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
          EmptyAllowed: 'Sie können das Feld leerlassen, um zum Standardnamen ("Gruppe A-Z") zurückzukehren.'
        }
      },
      Teams: {
        TableLabel: 'Mannschaften des Turniers',
        NoTeams: 'Keine Mannschaften vorhanden',
        Name: 'Name',
        Link: {
          Header: 'Anmeldung',
          Tooltip: 'Wenn eine Mannschaft aus einem Turnierplaner hinzugefügt wurde, können Sie hier zu der Anmeldung abspringen.'
        },
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
        RenameDisabledTooltip: 'Diese Mannschaft stammt aus einen Turnierplaner und kann daher nur dort umbenannt werden',
        Rename: {
          Button: 'Name ändern',
          Title: 'Mannschaft umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für die Mannschaft ein:',
          RequiredFeedback: 'Der Mannschaftsname darf nicht leer sein.'
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
          EnterNewName: 'Geben Sie den neuen Namen für das Dokument ein:'
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
            CurrencyInvalid: 'Die Währung muss angegeben werden und darf nicht leer sein.',
            HeaderInfo: 'Zusatz in der Kopfzeile (optional)',
            HeaderInfoTooltip: 'Sofern vorhanden, wird dieser Text zusätzlich zum Namen des Turniers angegeben.',
            HeaderInfoInvalid: 'Der angegebene Text ist ungültig.',
            SignatureLocation: 'Ort der Unterschrift (optional)',
            SignatureLocationInvalid: 'Der angegebene Ort ist ungültig.',
            SignatureRecipient: 'Name des Empfängers (optional)',
            SignatureRecipientTooltip:
              'Wenn angegeben, wird dieser Zahlungsempfänger im Bereich der Unterschrift angegeben. Als Standard steht dort "(Turnierleitung)".',
            SignatureRecipientInvalid: 'Der angegebene Text ist ungültig.',
            ShowPrimaryLogo: 'Hauptlogo abbilden (falls vorhanden)',
            ShowSecondaryLogo: 'zweites Logo abbilden (falls vorhanden)',
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
            OrganizerNameOverrideInvalid: 'Der angegebene Text ist ungültig.',
            TournamentNameOverride: 'Turniername (optional)',
            TournamentNameOverrideTooltip:
              'Standardmäßig wird auf dem Spielplan der Name des Turniers abgebildet. Wenn dieses Feld ausgefüllt wird, wird stattdessen der angegebene Text abgedruckt.',
            TournamentNameOverrideInvalid: 'Der angegebene Text ist ungültig.',
            VenueOverride: 'Spielort (optional)',
            VenueOverrideTooltip:
              'Standardmäßig wird auf dem Spielplan (sofern vorhanden) der Name der verknüpften Spielstätte abgebildet. Wenn dieses Feld ausgefüllt wird, wird stattdessen der angegebene Text abgedruckt.',
            VenueOverrideInvalid: 'Der angegebene Text ist ungültig.',
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
          HeaderCustomValueInvalid: 'Der Text darf nicht leer sein.',
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
          ShowPrimaryLogo: 'Hauptlogo darstellen',
          ShowSecondaryLogo: 'zweites Logo darstellen',
          SaveForbidden: 'Sie dürfen keine Änderungen an den Einstellungen machen.',
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
          EnterNewName: 'Geben Sie den neuen Namen für das Turnier ein:'
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
            HigherScoreLoses: {
              Label: 'Ergebnis invertieren:',
              Tooltip: 'Wenn aktiviert, ist diejenige Mannschaft, welche weniger Tore geschossen hat, der Gewinner vom Spiel.',
              Activated: 'Die Ergebnisse werden invertiert. Das heißt, die Mannschaft mit weniger Toren gewinnt.'
            },
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
          Title: 'Logos & Bilddateien'
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
          FolderNameInvalid: 'Der Name eines neuen Ordners darf nicht leer sein.',
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
          CannotDeleteGroupWithTeams: 'Entfernen Sie erst alle Mannschaften, um diese Gruppe zu löschen',
          MoveToOtherGroup: 'Verschieben in:',
          NoTeamsNotice: 'Keine Mannschaften vorhanden',
          NoGroupsNotice: 'Es existieren aktuell keine Gruppen',
          AddGroupButton: 'Neue Gruppe hinzufügen',
          ShuffleGroupsButton: 'Gruppen würfeln',
          GroupLimitReached: 'Die maximale Gruppenanzahl (26) ist erreicht.',
          GroupRename: {
            Button: 'Gruppennamen ändern',
            Title: 'Gruppennamen ändern',
            EnterNewName: 'Geben Sie den neuen Namen für die Gruppe ein:',
            EmptyAllowed: 'Sie können das Feld leerlassen, um zum Standardnamen ("Gruppe A-Z") zurückzukehren.'
          },
          TeamFromPlanningRealm: 'Diese Mannschaft stammt aus einen Turnierplaner: "{{planningRealm}}" ("{{tournamentClass}}")',
          TeamRename: {
            Button: 'Teamnamen ändern',
            Title: 'Teamnamen ändern',
            EnterNewName: 'Geben Sie den neuen Namen für die Mannschaft ein:'
          },
          AddTeam: {
            Button: 'Mannschaft',
            Title: 'Mannschaft hinzufügen',
            NewTeam: {
              Navigation: 'Neue Mannschaft',
              Explanation: 'Erstellen Sie eine neue Mannschaft, welche nur in diesem Turnier mitspielt.',
              TeamName: 'Mannschaftsname:',
              TeamNamePlaceholder: 'Neuen Mannschaftsnamen eingeben',
              TeamNameEmpty: 'Der Mannschaftsname darf nicht leer sein'
            },
            ImportTeam: {
              Navigation: 'Angemeldete Mannschaft'
            },
            Confirm: 'Hinzufügen',
            ConfirmWithSingle: '{{count}} Mannschaft hinzufügen',
            ConfirmWithCount: '{{count}} Mannschaften hinzufügen'
          }
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
        NameInvalid: 'Der Name einer neuen Spielstätte darf nicht leer sein.',
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
          RequiredFeedback: 'Der Name der Spielstätte darf nicht leer sein.'
        },
        Description: 'Beschreibung',
        NoDescription: 'Keine Beschreibung vorhanden',
        EditDescription: {
          Title: 'Beschreibung bearbeiten',
          InfoText: 'Geben Sie die Beschreibung für die Spielstätte ein (kann leer gelassen werden):',
          Confirm: 'Speichern'
        },
        AddressDetails: 'Adressdetails',
        AddressDetailsTooltip: 'Adressdetails sind z.B. Anschrift, Hausnummer oder Stadt.',
        NoAddressDetails: 'Keine Adressdetails vorhanden',
        EditAddressDetails: {
          Title: 'Adressdetails bearbeiten',
          HelpText: 'Beispiele für Adressdetails sind z.B. Straße, Hausnummer, PLZ oder Ort.',
          InvalidEntry: 'Ein bestehender Eintrag darf nicht leer sein.',
          NoEntries: 'Keine Einträge vorhanden',
          AddEntry: 'Zeile hinzufügen'
        },
        ExternalLinks: 'Externe Referenzen',
        ExternalLinksTooltip: 'Externe Links sind z.B. Links zu Kartenanbietern oder einer externen Webseite.',
        NoExternalLinks: 'Keine externen Referenzen vorhanden',
        EditExternalLinks: {
          Title: 'Externe Referenzen bearbeiten',
          HelpText: 'Beispiele für Externe Referenzen sind z.B. Links zu Ihrer Webseite oder einem Kartenanbieter.',
          InvalidEntry: 'Ein bestehender Eintrag darf nicht leer sein und es muss sich um einen gültigen HTTPS-Link handeln.',
          NoEntries: 'Keine Links vorhanden',
          AddEntry: 'Link hinzufügen'
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
    CreatePlanningRealm: {
      Title: 'Neuen Turnierplaner erstellen',
      Form: {
        Name: 'Name',
        NameInvalid: 'Der Name eines neuen Turnierplaners darf nicht leer sein.',
        NameValid: 'Dieser Name kann verwendet werden.'
      },
      OrganizationNotice: 'Es wird ein neuer Turnierplaner in der Organisation <span class="fw-bold">{{organizationName}}</span> angelegt.',
      Submit: 'Erstellen'
    },
    ViewPlanningRealm: {
      Pages: {
        TournamentClasses: 'Turnierklassen',
        InvitationLinks: 'Anmeldelinks',
        Labels: 'Label',
        Applications: 'Anmeldungen',
        Settings: 'Einstellungen'
      },
      Badges: {
        TournamentClassesCount: 'Turnierklassen',
        InvitationLinksCount: 'Anmeldelinks',
        LabelsCount: 'Labels'
      },
      NewTournamentClass: {
        Title: 'Neue Turnierklasse',
        InfoText: 'Geben Sie den Namen für die neue Turnierklasse ein:',
        RequiredFeedback: 'Der Name darf nicht leer sein',
        Confirm: 'Erstellen'
      },
      NewInvitationLink: {
        Title: 'Neuer Anmeldelink',
        InfoText:
          'Geben Sie den Namen für den neuen Anmeldelink ein. Der angegebene Name wird nicht auf dem Anmeldeformular sichtbar sein.',
        Alert: 'Das Anmeldeformular ist noch eine frühe Version und sollte noch nicht für den produktiven Einsatz verwendet werden!',
        RequiredFeedback: 'Der Name darf nicht leer sein',
        Confirm: 'Erstellen'
      },
      NewLabel: {
        Title: 'Neues Label',
        InfoText: 'Geben Sie den Namen für das neue Label ein:',
        Confirm: 'Erstellen'
      },
      AddApplication: {
        Title: 'Neue Anmeldung',
        TournamentClassRequired: 'Um eine Anmeldung zu erstellen, muss zunächst mindestens eine Turnierklasse vorhanden sein.',
        DialogTitle: 'Neue Anmeldung erstellen',
        Contact: 'Kontaktperson:',
        ContactInvalid: 'Eine Kontaktperson muss angegeben werden',
        ContactEmail: 'Kontakt E-Mail:',
        ContactEmailInvalid: 'Es muss sich um eine gültige E-Mailadresse handeln',
        ContactTelephone: 'Kontakt Telefon-Nr.:',
        Name: 'Verein / Mannschaftsname:',
        NameInvalid: 'Der Mannschaftsname muss angegeben werden',
        Teams: 'Mannschaften:',
        NoTeamsAlert: 'Es muss mindestens eine Mannschaft in einer Turnierklasse angegeben werden'
      },
      TournamentClasses: {
        Name: 'Name',
        InvitationLinkCount: 'Anmeldelinks',
        ApplicationCount: 'aktuelle Mannschaften',
        Rename: {
          Title: 'Turnierklasse umbenennen',
          EnterNewName:
            'Geben Sie den neuen Namen ein. Eine Änderung des Namens wird nach dem Speichern sofort auf allen Anmeldelinks, wo diese Turnierklasse verfügbar ist, sichtbar',
          RequiredFeedback: 'Der Name darf nicht leer sein'
        },
        DeleteNotPossible: 'Diese Turnierklasse kann nicht gelöscht werden, da es bereits Anmeldungen für diese Turnierklasse gibt.',
        NoTournamentClasses: 'Es sind aktuell keine Turnierklassen vorhanden.'
      },
      NoInvitationLinks: 'Es sind aktuell keine Anmeldelinks vorhanden.',
      InvitationLink: {
        PublicUrl: 'Link zum Anmeldeformular:',
        PublicUrlWarning:
          'Das Anmeldeformular kann nicht verwendet werden, weil das Formular deaktiviert ist oder weil keine Turniere hinzugefügt wurden.',
        Properties: {
          Name: 'Name:',
          ColorCode: 'Farbcode:',
          Title: 'Titel:',
          Description: 'Beschreibung:',
          ValidUntil: 'Anmeldeschluss:',
          ExternalLinks: 'Externe Links:',
          ContactPerson: 'Kontaktperson:',
          ContactEmail: 'Kontakt E-Mail:',
          ContactTelephone: 'Kontakt-Telefon:',
          Logos: 'Logos:'
        },
        ExpiredTooltip: 'Der Anmeldeschluss liegt in der Vergangenheit. Es sind aktuell keine Neuanmeldungen möglich.',
        EditProperties: 'Informationen ändern',
        EditPropertiesDialog: {
          Title: 'Anmeldelink bearbeiten',
          InternalInformation: 'Nur für angemeldete Benutzer sichtbar',
          PublicInformation: 'Die nachfolgenden Textfelder sind öffentlich sichtbar',
          HasValidUntilDate: 'Anmeldeschluss festlegen',
          NoExternalLinks: 'Keine Links vorhanden',
          ExternalLinkName: 'Anzeigename',
          ExternalLinkUrl: 'URL'
        },
        Tournaments: {
          Title: 'Turniere:',
          NoTournaments: 'Diesem Einladungslink sind aktuell keine Turniere zugeordnet.',
          Tournament: 'Turnier',
          AllowNewRegistrations: 'Aktiv',
          ActiveTooltip: 'Es können Mannschaften für dieses Turnier angemeldet werden.',
          InactiveTooltip: 'Es können derzeit keine weiteren Mannschaften für dieses Turnier angemeldet werden.',
          AllowNewRegistrationsTooltip: 'Sind aktuell weitere Anmeldungen möglich?',
          MaxTeamsPerRegistration: 'Limit pro Anmeldung',
          MaxTeamsPerRegistrationTooltip: 'Wie viele Mannschaften können mit einer Anmeldung angemeldet werden?',
          NumberOfTeams: 'aktuelle Mannschaften'
        },
        EditEntryDialog: {
          Title: 'Turnier-Eintrag bearbeiten',
          AllowNewRegistrations: 'Neue Anmeldungen erlauben',
          LimitTeamsPerRegistration: 'max. Anzahl Mannschaften pro Anmeldung begrenzen',
          MaxTeamsPerRegistration: 'Anzahl:'
        },
        AddTournament: 'Turnier hinzufügen',
        IsActive: {
          Title: 'Anmeldeformular:',
          Subtitle: 'Wenn das Anmeldeformular deaktiviert ist, kann es nicht mit dem obenstehenden Link aufgerufen werden.',
          Label: 'Anmeldeformular aktivieren'
        },
        AllClassesAdded: 'In diesem Link sind alle Turnierklassen enthalten',
        DeleteNotPossible:
          'Dieser Anmeldelink kann nicht gelöscht werden, da es bereits Anmeldungen gibt, welche über diesen Link durchgeführt wurden.',
        ShowApplications: 'Anmeldungen anzeigen'
      },
      Labels: {
        Name: 'Name',
        ColorPicker: 'Farbe wählen',
        Rename: {
          Title: 'Label umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen ein. Dieses Label bleibt weiterhin zugewiesen.',
          RequiredFeedback: 'Der Name darf nicht leer sein'
        },
        ChangeDescription: {
          Title: 'Beschreibung ändern',
          EnterNewName: 'Geben Sie die neue Beschreibung ein.',
          EmptyAllowed: 'Die Beschreibung darf leergelassen werden'
        },
        Description: 'Beschreibung',
        NoLabels: 'Es sind aktuell keine Labels vorhanden.',
        LabelsInfo: 'Labels können verwendet werden, um angemeldete Mannschaften zu kategorisieren und zu filtern.',
        DeleteWarning: {
          Title: 'Label löschen',
          Text: 'Wenn Sie ein Label löschen, wird dieses Label von allen Mannschaften entfernt, bei denen dieses Label aktuell zugewiesen ist. Dies kann nicht rückgängig gemacht werden!'
        }
      },
      SaveToViewApplications: 'Speichern Sie die offenen Änderungen, um die Anmeldungen zu sehen und neue Anmeldungen hinzuzufügen.',
      Applications: {
        FilterPanel: {
          SearchTerm: 'Suchbegriff (Tag, Name, E-Mail, ...)',
          TournamentClasses: 'Turnierklassen',
          InvitationLinks: 'Anmeldelinks',
          Labels: 'Label',
          Search: 'Suchen',
          Reset: 'Filter zurücksetzen'
        },
        TableHeader: {
          Tag: 'Tag',
          TagTooltip: 'Der Tag kann z.B. im E-Mailverkehr genutzt werden um eine eindeutige Referenz zu dieser Anmeldung zu haben.',
          CreatedAt: 'Erstellt am',
          TeamCount: 'Teams',
          Contact: 'Kontaktperson',
          ContactEmail: 'E-Mail',
          CopyAllEmailAddresses: 'Alle E-Mailadressen auf dieser Seite in die Zwischenablage kopieren',
          ContactTelephone: 'Telefon-Nr.',
          Comment: 'Bemerkung',
          CommentTooltip: 'Die Bemerkung wurde bei der Anmeldung mitgeschickt und kann nicht bearbeitet werden.',
          Notes: 'Notizen',
          NotesTooltip: 'In dieser Spalte können Sie eigene Notizen hinzufügen.',
          TournamentClass: 'Turnierklasse',
          TeamName: 'Mannschaftsname',
          Tournament: 'Turnier',
          Labels: 'Label'
        },
        NoSourceLinkTooltip: 'Diese Anmeldung wurde manuell angelegt',
        SourceLinkTooltip: 'Diese Anmeldung wurde mit dem Anmeldelink "{{name}}" angelegt',
        EditNotes: {
          Title: 'Notizen bearbeiten',
          InfoText: 'Geben Sie Ihre Notizen für die Anmeldung ein:',
          Confirm: 'Speichern'
        },
        NoLabels: 'keine Labels',
        NoLinkedTournament: 'kein Turnier verknüpft',
        RenameTeam: {
          Title: 'Mannschaft umbenennen',
          EnterNewName:
            'Geben Sie den neuen Mannschaftsnamen ein. Wenn diese Mannschaft einem Turnier hinzugefügt wurde, wird die Mannschaft im Turnier automatisch mit umbenannt.',
          RequiredFeedback: 'Der Mannschaftsname darf nicht leer sein'
        },
        HiddenTeamsTooltip: 'Diese Anmeldung beinhaltet weitere Mannschaften, welche nicht den Filterkriterien entsprechen',
        HiddenTeams: '{{count}} weitere Mannschaft(en) werden aufgrund Ihrer Filterkriterien nicht angezeigt.',
        HiddenTeamsShowAll: 'alle anzeigen',
        NoResults: 'Es gibt keine Anmeldungen, welche den angegebenen Filterkriterien entsprechen.'
      },
      Settings: {
        Rename: {
          Button: 'Umbenennen',
          Title: 'Turnierplaner umbenennen',
          EnterNewName: 'Geben Sie den neuen Namen für den Turnierplaner ein:'
        }
      },
      RbacWidget: {
        Info: 'Verwalten Sie, welche Nutzer auf diesen Turnierplaner zugreifen können und welche Aktionen sie durchführen können.'
      },
      DeleteWidget: {
        Title: 'Turnierplaner löschen',
        Info: 'Wenn Sie einen Turnierplaner löschen, werden alle verbundenen Turnierklassen, Anmeldelinks und alle Anmeldungen gelöscht. Alle verknüpften Turniere und Mannschaften bleiben bestehen, aber die Verknüpfung wird entfernt.',
        SuccessToast: {
          Title: 'Turnierplaner wurde gelöscht',
          Message: 'Ihr Turnierplaner wurde gelöscht.'
        }
      }
    },
    CreateApiKey: {
      Title: 'Neuen API-Schlüssel erstellen',
      Form: {
        Name: 'Name',
        NameInvalid: 'Der Name darf nicht leer sein.',
        NameValid: 'Dieser Name kann verwendet werden.',
        Description: 'Beschreibung',
        DescriptionInvalid: 'Die Beschreibung ist ungültig.',
        DescriptionValid: 'Diese Beschreibung kann verwendet werden.',
        Validity: 'Gültigkeit',
        Validity1: '1 Tag',
        Validity7: '1 Woche',
        Validity30: '30 Tage',
        Validity90: '90 Tage',
        Validity180: '180 Tage',
        Validity365: '365 Tage'
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
      EnterToConfirm:
        'Zur Bestätigung geben Sie bitte &quot;<span class="text-decoration-underline">{{text}}</span>&quot; in folgendes Textfeld ein:',
      ConfirmModalText: 'Bestätigen Sie den Löschvorgang. Dies kann <span class="fw-bold">nicht</span> rückgängig gemacht werden!',
      Delete: 'Löschen',
      DeleteConfirm: 'Löschen bestätigen'
    },
    VisibilitySelector: {
      Private: 'Privat',
      Public: 'Öffentlich'
    },
    ImageAlt: {
      PrimaryLogo: 'Hauptlogo',
      SecondaryLogo: 'zweites Logo',
      Banner: 'Banner'
    },
    ImageChooser: {
      Title: 'Bild hochladen oder auswählen',
      Remove: 'Bild entfernen',
      NoImages: 'Laden Sie Ihr erstes Bild hoch...',
      Upload: 'Hochladen',
      UploadFailed: 'Das Bild konnte nicht hochgeladen werden. Prüfen Sie die Maße und die maximale Dateigröße.',
      Constraints: {
        Logo: 'Das Bild muss quadratisch sein mit einer Auflösung zwischen 50x50 und 3000x3000 Pixel. Die maximale Dateigröße beträgt 8 MB.',
        Banner:
          'Das Bild muss mindestens 50px hoch sein und darf maximal 3000px breit sein. Das Seitenverhältnis muss zwischen 3:1 und 5:1 liegen. Die maximale Dateigröße beträgt 8 MB.'
      },
      DetailView: {
        Title: 'Hier sehen Sie die Detailinformationen zu folgendem Bild:',
        Name: 'Dateiname: {{value}}',
        CreatedAt: 'Hochgeladen am: {{value}}',
        FileSize: 'Dateigröße: {{value}} KB',
        Resolution: 'Auflösung: {{width}}x{{height}} px'
      }
    },
    MultiSelectFilter: {
      All: 'alle',
      Reset: 'Filter zurücksetzen',
      NoFilter: 'keine Filter verfügbar',
      NoFurtherFilter: 'keine weiteren Filter verfügbar'
    },
    LabelsSelect: {
      Title: 'Label wählen',
      NoLabels: 'In diesem Turnierplaner existieren keine Label',
      Clear: 'Alle entfernen'
    },
    ErrorPage: {
      Title: 'Fehler',
      ResourceNotFound: 'Die angeforderte Resource existiert nicht oder Sie\nhaben keine Berechtigung, diese Resource aufzurufen.',
      UnexpectedError:
        'Beim Verarbeiten der Anfrage ist ein unerwarteter Server-Fehler aufgetreten.\nLaden Sie die Seite neu und versuchen Sie es erneut.',
      ErrorDescription: 'Fehlerbeschreibung:'
    },
    SelectApplicationTeam: {
      Explanation: 'Wählen Sie eine angemeldete Mannschaft aus einem Turnierplaner:',
      LoadingPlanningRealms: 'Turnierplaner werden geladen',
      LoadingPlanningRealmsFailed: 'Fehler beim Laden der Turnierplaner',
      NoPlanningRealms: 'In dieser Organisation gibt es keine Turnierplaner',
      LoadingPlanningRealmDetail: 'Turnierplaner wird geladen',
      LoadingPlanningRealmDetailFailed: 'Fehler beim Laden des Turnierplaners',
      FilterExplanation: 'Suchen und wählen Sie die Mannschaften zum Hinzufügen:',
      FilterTooltip: 'Dieser Suchfilter wird synchronisiert mit dem Filter auf der Turnierplaner-Seite',
      LoadingApplications: 'Mannschaftsanmeldungen werden geladen',
      LoadingApplicationsFailed: 'Fehler beim Laden der Mannschaftsanmeldungen. Eventuell fehlen die erforderlichen Rechte',
      NoApplications:
        'Es gibt keine Anmeldungen, die den Suchkriterien entsprechen, oder alle Anmeldungen sind bereits einem Turnier zugeordnet.',
      TournamentClass: 'Turnierklasse',
      TeamName: 'Mannschaft',
      ApplicationDetails: 'Anmeldung'
    },
    RbacManagement: {
      Title: 'Zugriff verwalten',
      ButtonLabel: 'Verwalten',
      ButtonLabelAlternative: 'Zugriff',
      OffcanvasTitle: 'Rollenzuweisungen bearbeiten',
      Loading: 'Rollenzuweisungen werden geladen',
      NewRoleAssignment: 'Neue Rollenzuweisung',
      RoleName: {
        Owner: 'Besitzer',
        Contributor: 'Mitwirkender',
        Reader: 'Leser',
        Reporter: 'Turnierdurchführung',
        ApplicationOfficer: 'Mitwirkender für Anmeldungen',
        ApplicationReader: 'Leser für Anmeldungen'
      },
      RoleDescription: {
        Owner: 'Der Benutzer kann sämtliche Änderungen durchführen inkl. Änderungen an Zugriffsrechten.',
        Contributor: 'Der Benutzer kann sämtliche Änderungen durchführen ausgenommen Änderungen an Zugriffsrechten.',
        Reader: 'Der Benutzer kann sämtliche Informationen lesen aber keine Änderungen durchführen.',
        Reporter:
          'Der Benutzer kann Spielergebnisse eintragen und löschen sowie Änderungen an der Startgebühr und Priorität der Mannschaften durchführen.',
        ApplicationOfficer: 'Der Benutzer kann Anmeldungen im Turnierplaner anschauen, hinzufügen und bearbeiten.',
        ApplicationReader: 'Der Benutzer kann Anmeldungen im Turnierplaner anschauen aber keine Änderungen durchführen.'
      },
      PrincipalKind: {
        ApiKey: 'API-Schlüssel',
        User: 'Benutzer'
      },
      PrincipalNotFound: 'Der Prinzipal konnte nicht mehr gefunden werden. Wurde der Nutzer/API-Schlüssel gelöscht?',
      TotalCount: 'Gesamt:',
      Id: 'ID:',
      CreatedAt: 'Erstellt am:',
      Inherited: 'Vererbt durch:',
      InheritedTooltip: 'Diese Rollenzuweisung existiert implizit aufgrund der Zugehörigkeit zu einer anderen Resource',
      ScopeType: {
        Folder: {
          Tooltip: 'Ordner',
          NotInherited: 'Zuweisung liegt auf diesem Ordner'
        },
        Organization: {
          Tooltip: 'Organisation',
          NotInherited: 'Zuweisung liegt auf dieser Organisation'
        },
        PlanningRealm: {
          Tooltip: 'Turnierplaner',
          NotInherited: 'Zuweisung liegt auf diesem Turnierplaner'
        },
        Tournament: {
          Tooltip: 'Turnier',
          NotInherited: 'Zuweisung liegt auf diesem Turnier'
        },
        Venue: {
          Tooltip: 'Spielstätte',
          NotInherited: 'Zuweisung liegt auf dieser Spielstätte'
        }
      },
      DeletedSuccessToast: {
        Title: 'Rollenzuweisung gelöscht',
        Message: 'Die Rollenzuweisung wurde erfolgreich gelöscht.'
      },
      AddRoleAssignment: {
        Title: 'Rollenzuweisung hinzufügen',
        PreviousStep: 'Zurück',
        NextStep: 'Weiter',
        StepTitle: {
          SelectRole: 'Rolle selektieren',
          SelectPrincipal: 'Prinzipal selektieren'
        },
        AddAssignmentInfo:
          'Suchen Sie den gewünschten Nutzer oder API-Schlüssel anhand von E-Mailadresse bzw. API-Schlüssel-ID. Durch Klick auf "Suchen & hinzufügen" wird der spezifizierte Prinzipal gesucht und - sofern gefunden - eine entsprechende Rollenzuweisung erstellt.',
        SelectedRole: 'Gewählte Rolle:',
        SearchPrincipalPlaceholder: {
          ApiKey: 'ID des API-Schlüssels eingeben',
          User: 'Benutzername oder E-Mailadresse des Nutzers eingeben'
        },
        SearchPrincipalButton: 'Suchen & hinzufügen',
        CreatingRoleAssignment: 'Die Rollenzuweisung wird erstellt',
        CreateSuccessToast: {
          Title: 'Rollenzuweisung erstellt',
          Message: 'Die Rollenzuweisung wurde erfolgreich erstellt.'
        },
        PrincipalNotFoundToast: {
          Title: 'Rollenzuweisung konnte nicht erstellt werden',
          Message: 'Es existiert kein Prinzipal mit der angegebenen Api-Key-ID bzw. E-Mailadresse.'
        },
        AssignmentAlreadyExists: {
          Title: 'Rollenzuweisung konnte nicht erstellt werden',
          Message: 'Die angegebene Rolle ist bereits für den spezifizierten Prinzipal zugewiesen.'
        }
      }
    },
    IdWidget: {
      Title: 'Resourcen-ID',
      TitleShort: 'ID:',
      InfoText: 'Die ID kann verwendet werden, um mithilfe eines API-Keys auf diese Resource zuzugreifen.'
    },
    UpdatesCheck: {
      Loading: 'Release-Information wird von GitHub geladen...',
      Error: 'Beim Laden der Release-Information von GitHub ist ein Fehler aufgetreten',
      UpToDate: 'Die neuste Version <span class="fw-bold">{{version}}</span> ist installiert',
      Outdated: 'Es ist eine neue Version verfügbar: <span class="fw-bold">{{version}}</span>',
      ShowOnGitHub: 'Release auf GitHub öffnen'
    }
  }
};
