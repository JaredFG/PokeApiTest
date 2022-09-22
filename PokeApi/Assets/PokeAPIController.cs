using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;
public class PokeAPIController : MonoBehaviour
{

    public RawImage pokeRawImage;
    public TextMeshProUGUI pokeNameText, pokeNumText,pokeEXPText,pokeWeightText;
    public TextMeshProUGUI[] pokeTypeTextArray;

    private readonly string basePokeURL ="https://pokeapi.co/api/v2/";

    private void Start()
    {
        pokeRawImage.texture= Texture2D.blackTexture;

        pokeNameText.text="";
        pokeNumText.text="";
        pokeEXPText.text="";
        pokeWeightText.text="";

        foreach (TextMeshProUGUI pokeTypeText in pokeTypeTextArray)
        {
            pokeTypeText.text="";
        }
    }

    public void OnButtonRandomPokemon()
    {
        int randomPokeIndex = Random.Range(1,808);
        pokeRawImage.texture = Texture2D.blackTexture;
        pokeNameText.text = "Loading...";
        pokeNumText.text = "#" + randomPokeIndex;
        pokeEXPText.text = "...";
        pokeWeightText.text = "...";

        foreach (TextMeshProUGUI pokeTypeText in pokeTypeTextArray)
        {
            pokeTypeText.text = "";
        }
        StartCoroutine(GetPokemonAtIndex(randomPokeIndex));
    }

    IEnumerator GetPokemonAtIndex(int pokemonIndex)
    {
        string pokemonURL = basePokeURL + "pokemon/"+pokemonIndex.ToString();

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);
        yield return pokeInfoRequest.SendWebRequest();

        if(pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        string pokeName = pokeInfo["name"];
        string pokeEXP = pokeInfo["base_experience"];
        string pokeWeight = pokeInfo["weight"];
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];

        string[] pokeTypeNames = new string[pokeTypes.Count];

        for(int i = 0 , j = pokeTypes.Count -1; i< pokeTypes.Count;i++,j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }
        //GET SPRITE

        UnityWebRequest pokeSpriteRequest =UnityWebRequestTexture.GetTexture(pokeSpriteURL);

        yield return pokeSpriteRequest.SendWebRequest();

        if(pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }

        //SET UI
        pokeRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokeRawImage.texture.filterMode =FilterMode.Point;

        pokeEXPText.text = "EXP " + pokeEXP;
        pokeWeightText.text = "Weight " + pokeWeight;

        pokeNameText.text = CapitalizeFirstLetter(pokeName);
        
        for (int i =0;  i < pokeTypeNames.Length;i++)
        {
            pokeTypeTextArray[i].text=CapitalizeFirstLetter(pokeTypeNames[i]);

        }

    }
    private string CapitalizeFirstLetter(string str) {
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
