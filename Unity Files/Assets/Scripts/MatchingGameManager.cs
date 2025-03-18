using UnityEngine;
using System.Collections.Generic;

public class CardGrid3D : MonoBehaviour
{
    public GameObject cardPrefab; // The single card prefab
    public Material[] availableMaterials; // Array of 7 or more different materials
    public int rows = 4;  // Updated to 5 rows
    public int cols = 5;  // Updated to 4 columns
    public float spacingX = 2.0f;
    public float spacingY = 2.5f;
    private int no_of_materials;

    private List<Material> selectedMaterials = new List<Material>(); // Stores 10 selected materials
    private List<Material> cardMaterials = new List<Material>(); // Stores 20 shuffled materials

    void Start()
    {
        no_of_materials = (rows * cols) / 2;
        PrepareMaterials();
        RemoveAllChildren(); // Ensure old cards are cleared before generating new ones
        GenerateGrid();
    }

    public void PrepareMaterials()
    {
        // Step 1: Select 10 unique materials
        List<Material> tempMaterials = new List<Material>(availableMaterials);
        ShuffleList(tempMaterials);
        selectedMaterials = tempMaterials.GetRange(0, no_of_materials);

        // Step 2: Add each material twice (for pairing)
        cardMaterials.Clear();
        foreach (Material mat in selectedMaterials)
        {
            cardMaterials.Add(mat);
            cardMaterials.Add(mat);
        }

        // Step 3: Shuffle the final 20 card materials
        ShuffleList(cardMaterials);
    }

    public void GenerateGrid()
    {
        RemoveAllChildren();
        Vector3 startPos = transform.position;
        int cardIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (cardIndex >= cardMaterials.Count) return;

                Vector3 position = startPos + new Vector3(col * spacingX, -row * spacingY, 0);
                GameObject newCard = Instantiate(cardPrefab, position, Quaternion.identity, transform);

                // Apply material to the specific slot of the card's MeshRenderer
                MeshRenderer renderer = newCard.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material[] mats = renderer.materials; // Get all materials
                    mats[0] = cardMaterials[cardIndex]; // Assign the material to the first slot
                    renderer.materials = mats; // Apply the updated materials array
                }

                cardIndex++;
            }
        }
    }

    public void RemoveAllChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); // Destroys all child objects
        }
    }

    // Utility function to shuffle a list
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
