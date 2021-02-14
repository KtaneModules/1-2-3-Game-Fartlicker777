using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class TheOneTwoThreeGame : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] PlayerCards;
    public TextMesh[] TempText;
    public TextMesh NameSpace;
    public SpriteRenderer Players;
    public Sprite[] PlayersSprites;
    public SpriteRenderer[] LastPlayed;
    public Sprite[] PossibleCards;

    static int[][] NameCards = new int[13][] {
      new int[9] {3, 3, 1, 1, 2, 3, 1, 2, 2}, //Changyeop
      new int[9] {3, 3, 3, 2, 2, 2, 1, 1, 1}, //Eunji
      new int[9] {1, 2, 2, 2, 1, 3, 3, 1, 3}, //Gura
      new int[9] {3, 2, 2, 1, 2, 3, 1, 1, 3}, //Jinho
      new int[9] {2, 2, 1, 2, 3, 3, 1, 3, 1}, //Jungmoon
      new int[9] {1, 2, 3, 2, 1, 3, 2, 1, 3}, //Junseok
      new int[9] {2, 1, 1, 2, 3, 3, 3, 2, 1}, //Kyungran
      new int[9] {1, 1, 3, 2, 2, 2, 3, 3, 1}, //Minseo
      new int[9] {2, 1, 3, 1, 2, 3, 2, 1, 3}, //Minsoo
      new int[9] {3, 2, 2, 1, 1, 3, 3, 2, 1}, //Poong
      new int[9] {2, 2, 1, 1, 3, 3, 3, 2, 1}, //Sangmin
      new int[9] {1, 2, 1, 1, 3, 2, 3, 3, 2}, //Sunggyu
      new int[9] {1, 2, 1, 3, 3, 2, 1, 3, 2}  //Yuram
    };
    static int[][] ProfileCards = new int[12][] {
      new int[9] {2, 1, 1, 2, 3, 2, 3, 1, 3},
      new int[9] {2, 3, 1, 3, 1, 1, 2, 3, 2},
      new int[9] {3, 2, 2, 3, 3, 1, 1, 2, 1},
      new int[9] {3, 2, 1, 2, 1, 1, 3, 2, 3},
      new int[9] {1, 3, 1, 2, 2, 3, 2, 3, 1},
      new int[9] {3, 2, 3, 2, 1, 1, 2, 3, 1},
      new int[9] {1, 2, 2, 3, 1, 2, 3, 1, 3},
      new int[9] {2, 1, 3, 2, 3, 3, 1, 2, 1},
      new int[9] {2, 2, 2, 3, 3, 3, 1, 1, 1},
      new int[9] {1, 3, 2, 3, 1, 3, 2, 1, 2},
      new int[9] {3, 2, 3, 1, 2, 1, 1, 2, 3},
      new int[9] {3, 1, 1, 3, 1, 2, 2, 3, 2}
    };
    int[] OpponentsHand = new int[9];
    int NameSelector, ProfileSelector;
    int NameCardWins, ProfileCardWins, LastNameWin, LastProfileWin;
    int EnemyWins, YourWins;
    int CardOnePresses, CardTwoPresses, CardThreePresses;
    int Index;

    string[] Names = {"Changyeop", "Eunji", "Gura", "Jinho", "Jungmoon", "Junseok", "Kyungran", "Minseo", "Minsoo", "Poong", "Sangmin", "Sunggyu", "Yuram"};

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable Card in PlayerCards) {
            Card.OnInteract += delegate () { CardPress(Card); return false; };
        }
    }

    void Start () {
      LastPlayed[0].GetComponent<SpriteRenderer>().sprite = null;
      NameSelector = UnityEngine.Random.Range(0, 13);
      ProfileSelector = UnityEngine.Random.Range(0, 12);
      Players.GetComponent<SpriteRenderer>().sprite = PlayersSprites[ProfileSelector];
      NameSpace.text = Names[NameSelector];
      Debug.LogFormat("[The 1, 2, 3 Game {0}] The selected name is {1} and the selected profile is the {2} one.", moduleId, Names[NameSelector], NthChooser(ProfileSelector));
      //Determines which of the two decks is the opponent's hand
      for (int i = 0; i < 9; i++) {
        if (NameCards[NameSelector][i] > ProfileCards[ProfileSelector][i]) {
          NameCardWins++;
          LastNameWin = i;
        }
        else if (NameCards[NameSelector][i] < ProfileCards[ProfileSelector][i]) {
          ProfileCardWins++;
          LastProfileWin = i;
        }
      }
      if (NameCardWins > ProfileCardWins) {
        for (int i = 0; i < 9; i++) {
          OpponentsHand[i] = NameCards[NameSelector][i];
        }
        Debug.LogFormat("[The 1, 2, 3 Game {0}] The name's hand won, using their hand...", moduleId);
      }
      else if (NameCardWins < ProfileCardWins) {
        for (int i = 0; i < 9; i++) {
          OpponentsHand[i] = ProfileCards[ProfileSelector][i];
        }
        Debug.LogFormat("[The 1, 2, 3 Game {0}] The profile's hand won, using their hand...", moduleId);
      }
      else if (LastNameWin > LastProfileWin) {
        for (int i = 0; i < 9; i++) {
          OpponentsHand[i] = NameCards[NameSelector][i];
        }
        Debug.LogFormat("[The 1, 2, 3 Game {0}] There was a tie. The name's hand got to their final amount first. Using the name's hand...", moduleId);
      }
      else {
        for (int i = 0; i < 9; i++) {
          OpponentsHand[i] = ProfileCards[ProfileSelector][i];
        }
        Debug.LogFormat("[The 1, 2, 3 Game {0}] There was a tie. The profile's hand got to their final amount first. Using the profile's hand...", moduleId);
      }
    }

    string NthChooser (int x) {
      if (x == 0) {
        return "1st";
      }
      else if (x == 1) {
        return "2nd";
      }
      else if (x == 2) {
        return "3rd";
      }
      else {
        return (x + 1).ToString() + "th";
      }
    }

    void CardPress (KMSelectable Card) {
      if (Card == PlayerCards[0] && CardOnePresses != 3) {
        if (OpponentsHand[Index] != 1) {
          EnemyWins++;
          Debug.LogFormat("[The 1, 2, 3 Game {0}] You played a 1, and they played a {1}. You lost the round. Currently you are at {2} win(s) and they are at {3} win(s).", moduleId, OpponentsHand[Index], YourWins, EnemyWins);
        }
        else {
          Debug.LogFormat("[The 1, 2, 3 Game {0}] You played a 1, and they played a 1. You tied this round. Currently you are at {1} win(s) and they are at {2} win(s).", moduleId, YourWins, EnemyWins);
        }
        CardOnePresses++;
        LastPlayed[0].GetComponent<SpriteRenderer>().sprite = PossibleCards[0];
        LastPlayed[1].GetComponent<SpriteRenderer>().sprite = PossibleCards[OpponentsHand[Index] - 1];
        Index++;
      }
      else if (Card == PlayerCards[1] && CardTwoPresses != 3) {
        if (OpponentsHand[Index] > 2) {
          EnemyWins++;
          Debug.LogFormat("[The 1, 2, 3 Game {0}] You played a 2, and they played a 3. You lost the round. Currently you are at {1} win(s) and they are at {2} win(s).", moduleId, YourWins, EnemyWins);
        }
        else if (OpponentsHand[Index] < 2) {
          YourWins++;
          Debug.LogFormat("[The 1, 2, 3 Game {0}] You played a 2, and they played a 1. You won the round. Currently you are at {1} win(s) and they are at {2} win(s).", moduleId, YourWins, EnemyWins);
        }
        else {
          Debug.LogFormat("[The 1, 2, 3 Game {0}] You played a 2, and they played a 2. You tied this round. Currently you are at {1} win(s) and they are at {2} win(s).", moduleId, YourWins, EnemyWins);
        }
        CardTwoPresses++;
        LastPlayed[0].GetComponent<SpriteRenderer>().sprite = PossibleCards[1];
        LastPlayed[1].GetComponent<SpriteRenderer>().sprite = PossibleCards[OpponentsHand[Index] - 1];
        Index++;
      }
      else if (Card == PlayerCards[2] && CardThreePresses != 3) {
        if (OpponentsHand[Index] != 3) {
          YourWins++;
          Debug.LogFormat("[The 1, 2, 3 Game {0}] You played a 3, and they played a {1}. You win the round. Currently you are at {2} win(s) and they are at {3} win(s).", moduleId, OpponentsHand[Index], YourWins, EnemyWins);
        }
        else {
          Debug.LogFormat("[The 1, 2, 3 Game {0}] You played a 3, and they played a 3. You tied this round. Currently you are at {1} win(s) and they are at {2} win(s).", moduleId, YourWins, EnemyWins);
        }
        CardThreePresses++;
        LastPlayed[0].GetComponent<SpriteRenderer>().sprite = PossibleCards[2];
        LastPlayed[1].GetComponent<SpriteRenderer>().sprite = PossibleCards[OpponentsHand[Index] - 1];
        Index++;
      }
      if (Index == 9) {
        Check();
      }
    }

    void Check () {
      if (YourWins == 6) {
        Debug.LogFormat("[The 1, 2, 3 Game {0}] Congratulations, you have beat the shit out of {1}!", moduleId, Names[NameSelector]);
        GetComponent<KMBombModule>().HandlePass();
        moduleSolved = true;
      }
      else {
        Debug.LogFormat("[The 1, 2, 3 Game {0}] Unfortunately, you have beaten by {1}. Resetting to the initial state...", moduleId, Names[NameSelector]);
        GetComponent<KMBombModule>().HandleStrike();
        Index &= 0;
        CardOnePresses &= 0;
        CardTwoPresses &= 0;
        CardThreePresses &= 0;
        YourWins &= 0;
        EnemyWins &= 0;
      }
    }

    void Update () {
      TempText[0].text = "X " + (3-CardOnePresses).ToString();
      TempText[1].text = "X " + (3-CardTwoPresses).ToString();
      TempText[2].text = "X " + (3-CardThreePresses).ToString();
      TempText[3].text = YourWins.ToString();
      TempText[4].text = EnemyWins.ToString();
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} 1/2/3 to press that card.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
      if (Command.Length > 1) {
        yield return "sendtochaterror I don't understand!";
        yield break;
      }
      else {
        if (!("123".Contains(Command))) {
          yield return "sendtochaterror I don't understand!";
          yield break;
        }
        else {
          PlayerCards[int.Parse(Command) - 1].OnInteract();
        }
      }
      yield return new WaitForSecondsRealtime(.1f);
    }

    IEnumerator TwitchHandleForcedSolve () {
      while (!moduleSolved) {
        if (OpponentsHand[Index] == 1) {
          yield return ProcessTwitchCommand("2");
        }
        else if (OpponentsHand[Index] == 2) {
          yield return ProcessTwitchCommand("3");
        }
        else {
          yield return ProcessTwitchCommand("1");
        }
      }
    }
}
