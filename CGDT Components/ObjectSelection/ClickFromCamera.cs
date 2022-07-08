using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class ClickFromCamera : MonoBehaviour
{
    public float rotateSpeed = 20f;
    public float moveSpeed = 5f;
    public float scaleSpeed = 1f;

    private GameObject selected;
    private GameObject oldSelected;
    
    private Renderer selectionRenderer;
    private Renderer oldSelectionRenderer;

    private Camera _camera;

    private Color oldColor = Color.white;
    private Color newColor = Color.red;

    private string rootName;


    void Start()
    {   //gets the camera attached to the player
        _camera = GameObject.Find("Camera").GetComponent<Camera>();

        GameObject[] obs = (GameObject[]) Object.FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < obs.Length; i++)
        {
            rootName = obs[i].name;
            if (rootName != null)
            {
                ReadObjectTransform(rootName);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //if clicked
        {
            //cast a ray from the center of the camera's screen
            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0); //middle of camera view
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("raycast hit: " + hit.transform.gameObject.name);

                if (Input.GetMouseButtonDown(0)) //if clicked
                {
                    Debug.Log("clicked on: " + hit.transform.gameObject.name);

                    selected = hit.transform.gameObject;

                    if (selected != oldSelected) //if the object wasn't already selected
                    {
                        if (oldSelected != null) //if there was a previous object
                        { //returns previous selection to original color
                            oldSelectionRenderer = oldSelected.GetComponent<Renderer>();
                            oldSelectionRenderer.material.color = oldColor;
                        }
                        //change the selected object to red
                        selectionRenderer = selected.GetComponent<Renderer>();
                        selectionRenderer.material.color = newColor; //sets a new color

                        oldSelected = selected; //set the newly red object as the previously selected
                    }
                }
            }
        }

        //code to rotate selected object
        if (selected)
        {
            rootName = selected.name;
            SaveObjectTransform(rootName);

            //rotate right
            if (Input.GetKey(KeyCode.L))
            {
                selected.transform.RotateAround(_camera.transform.position, _camera.transform.up, rotateSpeed * Time.deltaTime);
            }
            //rotate left
            if (Input.GetKey(KeyCode.J))
            {
                selected.transform.RotateAround(_camera.transform.position, _camera.transform.up, -rotateSpeed * Time.deltaTime);
            }
            //move away from player
            if (Input.GetKey(KeyCode.I))
            {
               selected.transform.position += _camera.transform.forward * moveSpeed * Time.deltaTime; 
            }
            //move towards player
            if (Input.GetKey(KeyCode.K))
            {
                selected.transform.position += _camera.transform.forward * -moveSpeed * Time.deltaTime;
            }
            //move object up
            if (Input.GetKey(KeyCode.U))
            {
                selected.transform.position += _camera.transform.up * moveSpeed * Time.deltaTime;
            }
            //move object down
            if (Input.GetKey(KeyCode.M))
            {
               selected.transform.position += _camera.transform.up * -moveSpeed * Time.deltaTime;
            }
            
            Vector3 temp;
            //make object bigger
            if (Input.GetKey(KeyCode.O))
            {
                temp = selected.transform.localScale;
                temp.x += scaleSpeed * Time.deltaTime;
                temp.y += scaleSpeed * Time.deltaTime;
                temp.z += scaleSpeed * Time.deltaTime;
                selected.transform.localScale = temp;
            }
            //make object smaller
            if (Input.GetKey(KeyCode.P))
            {
                temp = selected.transform.localScale;
                temp.x -= scaleSpeed * Time.deltaTime;
                temp.y -= scaleSpeed * Time.deltaTime;
                temp.z -= scaleSpeed * Time.deltaTime;
                selected.transform.localScale = temp;
            }
            //objects rotate on x axis
            if (Input.GetKey(KeyCode.X))
            {
                selected.transform.RotateAround(selected.transform.position, selected.transform.right, Time.deltaTime * rotateSpeed);
            }
            //objects rotate on y axis
            if (Input.GetKey(KeyCode.Y))
            {
                selected.transform.RotateAround(selected.transform.position, selected.transform.up, Time.deltaTime * rotateSpeed);
            }
            //objects rotate on z axis
            if (Input.GetKey(KeyCode.Z))
            {
                selected.transform.RotateAround(selected.transform.position, selected.transform.forward, Time.deltaTime * rotateSpeed);
            }
        }



    }
    void SaveObjectTransform(string rootName)
    {
        Debug.Log("Saving: " + rootName);
        QuickSaveWriter.Create(rootName)
                        .Write("positionX", transform.position.x)
                        .Write("positionY", transform.position.y)
                        .Write("positionZ", transform.position.z)


                        .Write("rotationX", transform.eulerAngles.x)
                        .Write("rotationY", transform.eulerAngles.y)
                        .Write("rotationZ", transform.eulerAngles.z)

                        .Write("scaleX", transform.localScale.x)
                        .Write("scaleY", transform.localScale.y)
                        .Write("scaleZ", transform.localScale.z)

                        .Commit();

    }
    void ReadObjectTransform(string rootName)
    {
        QuickSaveReader reader = QuickSaveReader.Create(rootName);
        float positionX = reader.Read<float>("positionX");
        float positionY = reader.Read<float>("positionY");
        float positionZ = reader.Read<float>("positionZ");

        float rotationX = reader.Read<float>("rotationX");
        float rotationY = reader.Read<float>("rotationY");
        float rotationZ = reader.Read<float>("rotationZ");

        float scaleX = reader.Read<float>("scaleX");
        float scaleY = reader.Read<float>("scaleY");
        float scaleZ = reader.Read<float>("scaleZ");

        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

        transform.position = new Vector3(positionX, positionY, positionZ);

        transform.eulerAngles = new Vector3(rotationX, rotationY, rotationZ);
    }
}
