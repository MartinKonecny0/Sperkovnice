using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Vector3 = UnityEngine.Vector3;

public class ItemBar : MonoBehaviour
{
    private GameObject[] allIcons;
    public GameObject iconPrefab;
    private int numOfItems;
    private float X;
    private float Y;
    private float angle;
    private float r;

    public float[,] points = new float[0, 0];

    public float rotationSpeed;
    public float incrementPerItem;
    private float oneItemAngle;
    public float futureAngle;

    public float currentAngle;
    public float flattenAngle;
    private int selectedItemIndex;
    public void CreateBar(List<GameObject> allItems)
    {
        transform.Rotate(new Vector3(1, 0, 0), -flattenAngle);
        numOfItems = allItems.Count;
        CalculatePointsOnCircle();
        allIcons = new GameObject[numOfItems];
        for (int i = 0; i < numOfItems; i++)
        {
            Vector3 currIconPosition = new Vector3(points[i, 0], points[i, 1], 0);
            allIcons[i] = Instantiate(iconPrefab, transform);
            allIcons[i].transform.position = currIconPosition;
            allIcons[i].GetComponent<SpriteRenderer>().sprite = allItems[i].GetComponent<InteractableObject>().icon;
        }

        transform.Rotate(new Vector3(1, 0, 0), flattenAngle);
        selectedItemIndex = 0;
        UpdateSelected();
    }

    public void RotateBarRight(int newSelectedIndex)
    {
        selectedItemIndex = newSelectedIndex;
        futureAngle -= oneItemAngle;
        UpdateSelected();
    }
    public void RotateBarLeft(int newSelectedIndex)
    {
        selectedItemIndex = newSelectedIndex;
        futureAngle += oneItemAngle;
        UpdateSelected();
    }

    public void UpdateSelected()
    {
        
        int prevIndex = (selectedItemIndex - 1) % numOfItems;
        if (prevIndex < 0)
        {
            prevIndex = numOfItems - 1;
        }
        int nextIndex = (selectedItemIndex + 1) % numOfItems;

        GetCircularIndexSequence(allIcons.Length, selectedItemIndex);


        

    }


    public void GetCircularIndexSequence(int length, int index)
    {
        var result = new List<int>();
        result.Add(index);
        allIcons[index].GetComponent<Animator>().SetBool("isSelected", true);
        allIcons[index].GetComponent<Animator>().SetBool("isNearSelected", false);

        int rNext;
        int rNextNext;

        int lNext;
        int lNextNext;

        if (length > 1)
        {
            rNext = (index + 1) % length;
            result.Add(rNext);
            allIcons[rNext].GetComponent<Animator>().SetBool("isNearSelected", true);
            allIcons[rNext].GetComponent<Animator>().SetBool("isSelected", false);
        }

        if (length > 2)
        {
            lNext = (index - 1) % length;
            if (lNext < 0)
            {
                lNext += length;
            }
            allIcons[lNext].GetComponent<Animator>().SetBool("isNearSelected", true);
            allIcons[lNext].GetComponent<Animator>().SetBool("isSelected", false);

            result.Add(lNext);
        }
        if (length > 3)
        {
            rNextNext = (index + 2) % length;
            allIcons[rNextNext].GetComponent<Animator>().SetBool("isNearSelected", false);

            result.Add(rNextNext);
        }
        if (length > 4)
        {
            lNextNext = (index - 2) % length;
            if (lNextNext < 0)
            {
                lNextNext += length;
            }
            allIcons[lNextNext].GetComponent<Animator>().SetBool("isNearSelected", false);

            result.Add(lNextNext);
        }
    }

    public void HideBar()
    {
        if (allIcons != null)
        {
            foreach (var icon in allIcons)
            {
                if (icon != null)
                {
                    Destroy(icon);
                }
            }
        }
        currentAngle = 0;
        futureAngle = 0;

        numOfItems = 0;
        allIcons = null;
    }
    public void CalculatePointsOnCircle()
    {
        if (numOfItems > 0)
        {
            points = new float[numOfItems, 2];
            oneItemAngle = 2 * Mathf.PI / numOfItems;
            r = (numOfItems - 1) * incrementPerItem;
            GetFirstPoint(r);
            for (int pointCounter = 1; pointCounter < numOfItems; pointCounter++)
            {
                GetRotatedPoint( points[pointCounter - 1, 0], points[pointCounter - 1, 1], oneItemAngle);
                points[pointCounter, 0] = X;
                points[pointCounter, 1] = Y;
            }
        }

        //FlattenPoints();
    }

    void Update()
    {
        float difference = Mathf.Abs(currentAngle - futureAngle);
        rotationSpeed = difference * 5f;
        if (difference > 0)
        {
            if (difference < 0.003f)
            {
                currentAngle = futureAngle;
            }
            else if (currentAngle < futureAngle)
            {
                currentAngle += Time.deltaTime * rotationSpeed;
            }
            else
            {
                currentAngle -= Time.deltaTime * rotationSpeed;
            }



            transform.Rotate(new Vector3(1, 0, 0), -flattenAngle);
            CalculatePointsOnCircle();
            for (int i = 0; i < numOfItems; i++)
            {
                allIcons[i].transform.position = new Vector3(points[i, 0], points[i, 1], 0);
            }

            transform.Rotate(new Vector3(1, 0, 0), flattenAngle);
        }
    }

    public void GetFirstPoint(float r)
    {
        float firstX = transform.position.x;
        float firstY = transform.position.y - r;
        GetRotatedPoint(firstX, firstY, currentAngle);
        points[0, 0] = X;
        points[0, 1] = Y;

    }

    public void GetRotatedPoint(float x, float y, float angle)
    {
        x -= transform.position.x;
        y -= transform.position.y;
        X = x * Mathf.Cos(angle) - y * Mathf.Sin(angle);
        Y = x * Mathf.Sin(angle) + y * Mathf.Cos(angle);
        X += transform.position.x;
        Y += transform.position.y;
    }

    public void FlattenPoints()
    {
        //for (int pointCounter = 0; pointCounter < numberOfPoints; pointCounter++)
        //{
        //    if (points[pointCounter, 1] - b < 0)
        //    {
        //        points[pointCounter, 1] = points[pointCounter, 1] + Mathf.Pow(r - Mathf.Abs(points[pointCounter, 0] - transform.position.x), power) * flatten;
        //    }
        //    else
        //    {
        //        points[pointCounter, 1] = points[pointCounter, 1] - Mathf.Pow(r - Mathf.Abs(points[pointCounter, 0] - transform.position.x), power) * flatten;
        //    }
        //}
    }
}
