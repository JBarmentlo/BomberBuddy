/*
 * Copyright(c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files(the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script makes sure that a bomb can be laid down at the player's feet without causing buggy movement when the player walks away.
/// It disables the trigger on the collider, essentially making the object solid.
/// </summary>
public class DisableTriggerOnPlayerExit : MonoBehaviour
{
    private Collider    collid;
    // private bool        firstFrame = true;
    // private List<Collider>  ignoredColliders;

    IEnumerator EnableCollision()
    {
        yield return new WaitForSeconds(0.1f);
        collid.isTrigger = false; // Disable the trigger
    }
    void Start()
    {
        Collider[] G = GetComponents<Collider>();
        collid = G[0];
        StartCoroutine(EnableCollision());
    }


    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        { // When the player exits the trigger area
            // Debug.Log("Trigger Exit");
            // Physics.IgnoreCollision(collid, other, false);
            // collid.isTrigger = false; // Disable the trigger
        }
    }  
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        { // When the player exits the trigger area
            // Debug.Log("Trigger Enter");
            Physics.IgnoreCollision(collid, other, true);
            // GetComponent<Collider>().isTrigger = false; // Disable the trigger
        }
    }  

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Colision ENter");
            Physics.IgnoreCollision(collid, other.collider, true);
        }
    }

    void OnCollision(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Colision");
            // Physics.IgnoreCollision(collid, other.collider);
        }
    }

    void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Colision EXIT");
            Physics.IgnoreCollision(collid, other.collider, false);
        }
    }
}
