using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour {

    public static string saveData;

    private bool isSave = false;
    GameObject tmp = null;

    void Update () {
        if (Input.GetKeyDown(KeyCode.S) && !isSave)
        {
            isSave = true;
            saveData = "";
            foreach(Bricks _brick in BrickManager.brickList)
            {
                saveData += _brick.location.x + "_" + _brick.location.y + "_" + _brick.location.z + ":"
                    + _brick.rotation.x + "_" + _brick.rotation.y + "_" + _brick.rotation.z + "_" + _brick.rotation.w + ":"
                    + _brick.color + ":"
                    + _brick.type + "&";
                Destroy(_brick.gameObject);
            } // saveData format = x_y_z:x_y_z_w:color:type&....
            saveData = saveData.Substring(0, saveData.Length-1);
            BrickManager.brickList.Clear();
            print("...Saved!");
        }

        if (Input.GetKeyDown(KeyCode.L) && isSave)
        {
            isSave = false;
            string[] loadData = saveData.Split('&');
            for (int i = 0; i < loadData.Length; i++)
            {
                string[] property = loadData[i].Split(':');
                // 0 : Location / 1 : Rotation / 2 : Color / 3 : Type

                string[] savedLocation = property[0].Split('_');
                Vector3 location = new Vector3(float.Parse(savedLocation[0]), float.Parse(savedLocation[1]), float.Parse(savedLocation[2]));
                string[] savedRocation = property[1].Split('_');
                Quaternion rotation = new Quaternion(float.Parse(savedRocation[0]), float.Parse(savedRocation[1]), float.Parse(savedRocation[2]), float.Parse(savedRocation[3]));                
                int type = int.Parse(property[3]);

                GameObject bricks = Instantiate(BrickManager.getBrick(type), location, rotation);
                bricks.GetComponent<Renderer>().material = BrickManager.getColor(property[2]);
            }
            print("...Loaded!");
        }
    }
}
