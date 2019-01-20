﻿using System.Collections.Generic;
using UnityEngine;

public class ResultsLabel : MonoBehaviour
{
    public static ResultsLabel instance;

    public GameObject cursor;

    public Transform labelPrefab;

    [HideInInspector]
    public Transform lastLabelPlaced;

    [HideInInspector]
    public TextMesh lastLabelPlacedText;

    private void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;
    }

    /// <summary>
    /// Instantiate a Label in the appropriate location relative to the Main Camera.
    /// </summary>
    public void CreateLabel()
    {
        lastLabelPlaced = Instantiate(labelPrefab, cursor.transform.position, transform.rotation);

        lastLabelPlacedText = lastLabelPlaced.GetComponent<TextMesh>();

        // Change the text of the label to show that has been placed
        // The final text will be set at a later stage
        lastLabelPlacedText.text = "Analysing...";
    }

    /// <summary>
    /// Set the Tags as Text of the last Label created. 
    /// </summary>
    public void SetTagsToLastLabel(string label)
    {
        lastLabelPlacedText = lastLabelPlaced.GetComponent<TextMesh>();

        // At this point we go through all the tags received and set them as text of the label
<<<<<<< HEAD
        lastLabelPlacedText.text = "I see: " + label;
=======
        lastLabelPlacedText.text = label;
>>>>>>> parent of f843d87... Trigger sound and delete text after 8 seconds
    }
}