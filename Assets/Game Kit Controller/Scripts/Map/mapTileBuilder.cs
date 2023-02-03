using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class mapTileBuilder : MonoBehaviour
{
	public Transform mapPartParent;

	public int mapPartBuildingIndex;
	public int mapPartFloorIndex;
	public int mapPartIndex;

	public List<Transform> verticesPosition = new List<Transform> ();
	public List<GameObject> eventTriggerList = new List<GameObject> ();
	public List<GameObject> extraMapPartsToActive = new List<GameObject> ();
	public mapCreator mapManager;
	public float mapPartRendererOffset;
	public Vector2 newPositionOffset;
	public bool mapPartEnabled = true;
	public bool useOtherColorIfMapPartDisabled;
	public Color colorIfMapPartDisabled;

	public bool showGizmo = true;
	public bool showEnabledTrigger = true;
	public bool showVerticesDistance;
	public Color mapPartMaterialColor = Color.white;
	public Vector3 cubeGizmoScale = Vector3.one;
	public Color gizmoLabelColor = Color.white;
	public Color mapLinesColor = Color.yellow;
	public bool useHandleForVertex;
	public float handleRadius = 0.1f;

	public bool showVertexHandles;

	public List<GameObject> textMeshList = new List<GameObject> ();
	public Vector3 center;

	public string mapPartName;

	public string internalName;

	public bool generate3dMapPartMesh;
	public bool onlyUse3dMapPartMesh;
	public bool generate3dMeshesShowGizmo;
	public GameObject mapPart3dGameObject;

	public Vector3 mapPart3dOffset;
	public float mapPart3dHeight = 1;

	public bool mapPart3dMeshCreated;

	public GameObject mapTileRenderer;
	public MeshFilter mapTileMeshFilter;

	public bool mapTileCreated;
	public MeshRenderer mainMeshRenderer;

	GameObject eventTriggerParent;
	GameObject textMeshParent;

	MeshRenderer mapPart3dMeshRenderer;
	Color originalMapPart3dMeshRendererColor;

	Vector3 current3dOffset;

	public void createMapTileElement ()
	{
		if (!mapTileCreated) {

			calculateMapTileMesh ();

			generateMapPart3dMesh (mapPart3dHeight);

			mapTileCreated = true;
		}

		if (!mapPartEnabled) {
			if (useOtherColorIfMapPartDisabled) {
				setWallRendererMaterialColor (colorIfMapPartDisabled);

				enableOrDisableTextMesh (false);
			} else {
				disableMapPart ();
			}

			enableOrDisableMapPart3dMesh (false);
		} else {
			if (onlyUse3dMapPartMesh) {
				if (mapTileRenderer != null) {
					if (mapTileRenderer.activeSelf) {
						mapTileRenderer.SetActive (false);
					}
				}
			}
		}
	}

	public void calculateMapTileMesh ()
	{
		List<Vector2> vertices2D = new List<Vector2> ();

		int verticesPositionCount = verticesPosition.Count;

		Vector3 currentPosition = Vector3.zero;

		for (int i = 0; i < verticesPositionCount; i++) {
			currentPosition = verticesPosition [i].localPosition;

			vertices2D.Add (new Vector2 (currentPosition.x, currentPosition.y));
		}

		// Use the triangulator to get indices for creating triangles
		GKC_Triangulator_Utils tr = new GKC_Triangulator_Utils (vertices2D);
		int[] triangles = tr.Triangulate ();

		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[vertices2D.Count];

		int verticesLength = vertices.Length;

		Vector2 current2DPosition = Vector2.zero;

		for (int i = 0; i < verticesLength; i++) {
			current2DPosition = vertices2D [i];

			vertices [i] = new Vector3 (current2DPosition.x, current2DPosition.y, 0);
		}

		// Create the mesh
		Mesh msh = new Mesh ();
		msh.vertices = vertices;
		msh.triangles = triangles;
		msh.RecalculateNormals ();
		msh.RecalculateBounds ();

		// Set up game object with mesh;
		if (mapTileRenderer == null) {
			mapTileRenderer = new GameObject ();

			mapTileRenderer.transform.SetParent (transform);
			mapTileRenderer.layer = LayerMask.NameToLayer (mapManager.mapLayer);

			mapTileRenderer.transform.localPosition = Vector3.zero;
			mapTileRenderer.transform.localPosition -= mapTileRenderer.transform.forward * mapPartRendererOffset;
			mapTileRenderer.transform.localRotation = Quaternion.identity;

			mapTileRenderer.name = "Map Tile Renderer";

			mainMeshRenderer = mapTileRenderer.AddComponent (typeof(MeshRenderer)) as MeshRenderer;

			Material newMaterial = new Material (mapManager.floorMaterial);

			mainMeshRenderer.material = newMaterial;
		}

		if (mapTileMeshFilter == null) {
			mapTileMeshFilter = mapTileRenderer.AddComponent (typeof(MeshFilter)) as MeshFilter;
		}

		mapTileMeshFilter.mesh = msh;

		setWallRendererMaterialColor (mapPartMaterialColor);

		mapTileCreated = true;
	}

	public void removeMapTileRenderer ()
	{
		if (mapTileRenderer != null) {
			DestroyImmediate (mapTileRenderer);
		}

		if (mapTileMeshFilter != null) {
			DestroyImmediate (mapTileMeshFilter);
		}

		if (mainMeshRenderer != null) {
			DestroyImmediate (mainMeshRenderer);
		}

		mapTileCreated = false;

		updateComponent ();
	}

	public void setWallRendererMaterialColor (Color newColor)
	{
		if (mainMeshRenderer != null) {
			if (Application.isPlaying) {
				mainMeshRenderer.material.color = newColor;
			} else {
				mainMeshRenderer.sharedMaterial.color = newColor;
			}
		}
	}

	public void set3dMapPartMaterialColor (Color newColor)
	{
		if (mapPart3dMeshRenderer != null) {
			mapPart3dMeshRenderer.material.color = newColor;
		}
	}

	public void setOriginal3dMapPartMaterialColor ()
	{
		if (mapPart3dMeshRenderer != null) {
			mapPart3dMeshRenderer.material.color = originalMapPart3dMeshRendererColor;
		}
	}

	public void enableMapPart ()
	{
		if (!mapPartEnabled) {
			if (!onlyUse3dMapPartMesh) {
				if (!mapTileRenderer.activeSelf) {
					mapTileRenderer.SetActive (true);
				}
			}

			enableOrDisableTextMesh (true);

			mapPartEnabled = true;

			mapManager.enableOrDisableSingleMapIconByMapPartIndex (mapPartBuildingIndex, mapPartIndex, mapPartFloorIndex, true);

			enableOrDisableEventTriggerList (false);

			int extraMapPartsToActiveCount = extraMapPartsToActive.Count;

			for (int i = 0; i < extraMapPartsToActiveCount; i++) {
				if (extraMapPartsToActive [i] != null) {
					extraMapPartsToActive [i].GetComponent<mapTileBuilder> ().enableMapPart ();
				}
			}

			enableOrDisableMapPart3dMesh (true);
		}

		mapManager.setCurrentMapPartIndex (mapPartIndex);
	}

	public void enableOrDisableEventTriggerList (bool state)
	{
		int eventTriggerListCount = eventTriggerList.Count;

		for (int i = 0; i < eventTriggerListCount; i++) {
			if (eventTriggerList [i] != null) {
				if (eventTriggerList [i].activeSelf != state) {
					eventTriggerList [i].SetActive (state);
				}
			}
		}
	}

	public void disableMapPart ()
	{
		if (mapTileRenderer.activeSelf) {
			mapTileRenderer.SetActive (false);
		}

		enableOrDisableTextMesh (false);

		mapPartEnabled = false;
	}

	public void setMapPartEnabledState (bool state)
	{
		mapPartEnabled = state;
	}

	public void enableOrDisableTextMesh (bool state)
	{
		if (textMeshList.Count > 0) {
			int textMeshListCount = textMeshList.Count;

			for (int i = 0; i < textMeshListCount; i++) {
				if (textMeshList [i] != null && textMeshList [i].activeSelf != state) {
					textMeshList [i].SetActive (state);
				}
			}

			if (useOtherColorIfMapPartDisabled) {
				if (state) {
					setWallRendererMaterialColor (mapPartMaterialColor);
				} else {
					setWallRendererMaterialColor (colorIfMapPartDisabled);
				}
			}
		}
	}

	public void addEventTriggerToActive ()
	{
		if (eventTriggerList.Count == 0 || eventTriggerParent == null) {
			eventTriggerParent = new GameObject ();

			eventTriggerParent.name = "Triggers Parent";

			eventTriggerParent.transform.SetParent (transform);
			eventTriggerParent.transform.localPosition = Vector3.zero;
			eventTriggerParent.transform.localRotation = Quaternion.identity;
		}

		mapPartEnabled = false;

		GameObject trigger = new GameObject ();

		trigger.AddComponent<BoxCollider> ().isTrigger = true;

		trigger.AddComponent<eventTriggerSystem> ().setSimpleFunctionByTag ("enableMapPart", gameObject, "Player");

		trigger.transform.SetParent (eventTriggerParent.transform);

		if (mapManager.useRaycastToPlaceElements) {
			Camera currentCameraEditor = GKC_Utils.getCameraEditor ();

			if (currentCameraEditor != null) {
				Vector3 editorCameraPosition = currentCameraEditor.transform.position;
				Vector3 editorCameraForward = currentCameraEditor.transform.forward;

				RaycastHit hit;

				if (Physics.Raycast (editorCameraPosition, editorCameraForward, out hit, Mathf.Infinity, mapManager.layerToPlaceElements)) {
					trigger.transform.position = hit.point + Vector3.up * 0.1f;
				}
			}
		} else {
			trigger.transform.localPosition = Vector3.zero;
		}

		trigger.transform.rotation = Quaternion.identity;

		if (mapManager.mapPartEnabledTriggerScale != Vector3.zero) {
			trigger.transform.localScale = mapManager.mapPartEnabledTriggerScale;
		}

		trigger.layer = LayerMask.NameToLayer ("Ignore Raycast");
		trigger.name = "Map Part Enabled Trigger " + (eventTriggerList.Count + 1);

		eventTriggerList.Add (trigger);

		updateComponent ();
	}

	public void setMapPartEnabledStateFromEditor (bool state)
	{
		mapPartEnabled = state;

		updateComponent ();
	}

	public void removeEventTrigger (int eventTriggerIndex)
	{
		GameObject currentEventTrigger = eventTriggerList [eventTriggerIndex];

		if (currentEventTrigger != null) {
			DestroyImmediate (currentEventTrigger);
		}

		eventTriggerList.RemoveAt (eventTriggerIndex);

		if (eventTriggerList.Count == 0) {
			if (eventTriggerParent != null) {
				DestroyImmediate (eventTriggerParent);
			}
		}

		updateComponent ();
	}

	public void removeAllEventTriggers ()
	{
		for (int i = 0; i < eventTriggerList.Count; i++) {
			if (eventTriggerList [i] != null) {
				DestroyImmediate (eventTriggerList [i]);
			}
		}

		eventTriggerList.Clear ();

		if (eventTriggerParent != null) {
			DestroyImmediate (eventTriggerParent);
		}

		updateComponent ();
	}

	public void addMapPartTextMesh ()
	{
		GameObject textMesh = GKC_Utils.getLoadAssetAtPath ("Assets/Game Kit Controller/Prefabs/Map System/mapPartTextMesh.prefab");

		if (textMesh != null) {

			if (textMeshList.Count == 0 || textMeshParent == null) {
				textMeshParent = new GameObject ();
				textMeshParent.name = "Text Mesh Parent";
				textMeshParent.transform.SetParent (transform);
				textMeshParent.transform.localPosition = Vector3.zero;
				textMeshParent.transform.localRotation = Quaternion.identity;
			}

			textMesh = (GameObject)Instantiate (textMesh, transform.position, Quaternion.identity, textMeshParent.transform);

			Vector3 textMeshPosition = transform.position;

			if (verticesPosition.Count > 0) {
				textMeshPosition = center;
			}

			textMesh.transform.position = textMeshPosition + textMesh.transform.forward;
			textMesh.transform.localRotation = Quaternion.identity;
			textMesh.name = "Map Part Text Mesh " + (textMeshList.Count + 1).ToString ("000");

			textMeshList.Add (textMesh);
		} else {
			print ("Prefab not found");
		}
	}

	public void removeTextMesh (int textMeshIndex)
	{
		GameObject currentTextMesh = textMeshList [textMeshIndex];

		if (currentTextMesh != null) {
			DestroyImmediate (currentTextMesh);
		}

		textMeshList.RemoveAt (textMeshIndex);

		if (textMeshList.Count == 0) {
			if (textMeshParent != null) {
				DestroyImmediate (textMeshParent);
			}
		}

		updateComponent ();
	}

	public void removeAllTextMesh ()
	{
		for (int i = 0; i < textMeshList.Count; i++) {
			if (textMeshList [i] != null) {
				DestroyImmediate (textMeshList [i]);
			}
		}

		textMeshList.Clear ();

		if (textMeshParent != null) {
			DestroyImmediate (textMeshParent);
		}

		updateComponent ();
	}

	public void addNewVertex (int insertAtIndex)
	{
		GameObject newTransform = new GameObject ();
		newTransform.transform.SetParent (transform);

		newTransform.transform.localRotation = Quaternion.identity;

		if (verticesPosition.Count > 0) {
			Vector3 lastPosition = verticesPosition [verticesPosition.Count - 1].localPosition;
			newTransform.transform.localPosition = new Vector3 (lastPosition.x + newPositionOffset.x + 1, lastPosition.y + newPositionOffset.y + 1, lastPosition.z);
		} else {
			newTransform.transform.localPosition = new Vector3 (newPositionOffset.x + 1, newPositionOffset.y + 1, 0);
		}

		newTransform.name = (verticesPosition.Count + 1).ToString ("000");

		if (insertAtIndex > -1) {
			if (verticesPosition.Count > 0) {
				Vector3 vertexPosition = verticesPosition [insertAtIndex].localPosition;
				newTransform.transform.localPosition = new Vector3 (vertexPosition.x + newPositionOffset.x + 1, vertexPosition.y + newPositionOffset.y + 1, vertexPosition.z);
			}

			verticesPosition.Insert (insertAtIndex + 1, newTransform.transform);
			newTransform.transform.SetSiblingIndex (insertAtIndex + 1);

			renameAllVertex ();
		} else {
			verticesPosition.Add (newTransform.transform);
		}

		updateComponent ();
	}

	public void removeVertex (int vertexIndex)
	{
		Transform currentVertex = verticesPosition [vertexIndex];

		if (currentVertex != null) {
			DestroyImmediate (currentVertex.gameObject);
		}

		verticesPosition.RemoveAt (vertexIndex);

		updateComponent ();
	}

	public void removeAllVertex ()
	{
		for (int i = 0; i < verticesPosition.Count; i++) {
			if (verticesPosition [i] != null) {
				DestroyImmediate (verticesPosition [i].gameObject);
			}
		}

		verticesPosition.Clear ();

		updateComponent ();
	}

	public void renameAllVertex ()
	{
		for (int i = 0; i < verticesPosition.Count; i++) {
			if (verticesPosition [i] != null) {
				verticesPosition [i].name = (i + 1).ToString ("000");
			}
		}

		updateComponent ();
	}

	public void reverVertexOrder ()
	{
		verticesPosition.Reverse ();

		updateComponent ();
	}

	public void setInternalName (string nameToConfigure)
	{
		internalName = nameToConfigure;

		renameMapPart ();
	}

	public void renameMapPart ()
	{
		string newName = internalName;

		if (mapPartName != "") {
			newName += " (" + mapPartName + ")";
		}

		gameObject.name = newName;

		updateComponent ();
	}

	public void updateMapPart3dMeshPositionFromEditor ()
	{
		updateMapPart3dMeshPosition (mapPart3dOffset);
	}

	public void updateMapPart3dMeshPosition (Vector3 offset)
	{
		if (mapPart3dMeshCreated) {
			mapPart3dGameObject.transform.position = center + offset;
		}
	}

	public void enableOrDisableMapPart3dMesh (bool state)
	{
		if (mapPart3dMeshCreated) {
			if (mapPart3dGameObject.activeSelf != state) {
				mapPart3dGameObject.SetActive (state);
			}
		}
	}

	public void removeMapPart3dMesh ()
	{
		if (mapPart3dMeshCreated && mapPart3dGameObject != null) {
			DestroyImmediate (mapPart3dGameObject);

			mapPart3dMeshCreated = false;
		}
	}

	public void removeMapPart3dMeshFromEditor ()
	{
		if (mapPart3dMeshCreated && mapPart3dGameObject != null) {
			DestroyImmediate (mapPart3dGameObject);

			mapPart3dMeshCreated = false;

			updateComponent ();
		}
	}

	public void setGenerate3dMapPartMeshState (bool state)
	{
		generate3dMapPartMesh = state;
	}

	public void setGenerate3dMapPartMeshStateFromEditor (bool state)
	{
		generate3dMapPartMesh = state;

		updateComponent ();
	}

	public void generateMapPart3dMeshFromEditor ()
	{
		generateMapPart3dMesh (mapPart3dHeight);

		updateComponent ();
	}

	public void generateMapPart3dMesh (float meshHeight)
	{
		if ((!generate3dMapPartMesh && (!generate3dMapPartMesh && !mapManager.generateFull3dMapMeshes)) || !mapManager.generate3dMeshesActive) {
			return;
		}
			
		removeMapPart3dMesh ();

		mapPart3dMeshCreated = true;

		mapPart3dGameObject = new GameObject ();

		mapPart3dGameObject.name = gameObject.name + " - 3d Mesh";

		mapPart3dGameObject.isStatic = true;

		mapPart3dGameObject.layer = LayerMask.NameToLayer (mapManager.mapLayer);

		mapPart3dGameObject.transform.SetParent (transform);

		mapPart3dGameObject.transform.position = center + mapPart3dOffset;

		MeshRenderer mapPart3dRenderer = mapPart3dGameObject.AddComponent<MeshRenderer> ();

		mapPart3dMeshRenderer = mapPart3dRenderer;

		MeshFilter mapPart3dMeshFilter = mapPart3dGameObject.AddComponent<MeshFilter> ();

		Mesh mapPart3dMesh = mapPart3dMeshFilter.mesh;

		mapPart3dRenderer.material = mapManager.mapPart3dMeshMaterial;
		originalMapPart3dMeshRendererColor = mapPart3dMeshRenderer.material.color;

		mapPart3dMesh.Clear ();
		mapPart3dMesh.ClearBlendShapes ();

		int verticesPositionCount = verticesPosition.Count;


		Vector3 position1 = verticesPosition [0].position;
		Vector3 position2 = verticesPosition [1].position;

		Vector3 direction1 = center - position1;
		direction1 = direction1 / direction1.magnitude;
		Vector3 direction2 = center - position2;
		direction2 = direction2 / direction2.magnitude;

		float angle1 = Vector3.Angle (direction1, Vector3.forward);
		float angle2 = Vector3.Angle (direction2, Vector3.forward);

//		print (gameObject.name + " " + angle1 + " " + angle2);

		if (angle1 < angle2) {
			verticesPosition.Reverse ();
		} 
			
		Vector3[] downCorners = new Vector3[verticesPositionCount];

		int downCornersLength = downCorners.Length;

		for (int x = 0; x < downCornersLength; x++) {
			downCorners [x] = mapPart3dGameObject.transform.InverseTransformPoint (verticesPosition [x].position);
		}

		Vector3[] topCorners = new Vector3[verticesPositionCount];

		downCorners.CopyTo (topCorners, 0);

		int topCornersLength = topCorners.Length;

		for (int x = 0; x < topCornersLength; x++) {
			topCorners [x] += new Vector3 (0, meshHeight, 0);
		}

		Vector3[] cornersCombined = new Vector3[downCorners.Length + topCorners.Length];

		downCorners.CopyTo (cornersCombined, 0);

		topCorners.CopyTo (cornersCombined, downCorners.Length);

		mapPart3dMesh.vertices = cornersCombined;

		List<Vector2> downVertices2D = new List<Vector2> ();

		for (int i = 0; i < downCorners.Length; i++) {
			downVertices2D.Add (new Vector2 (downCorners [i].x, downCorners [i].z));
		}

		GKC_Triangulator_Utils donwTr = new GKC_Triangulator_Utils (downVertices2D);
		int[] downIndices = donwTr.Triangulate ();


		int[] reverseDownIndices = new int[downIndices.Length];

		downIndices.CopyTo (reverseDownIndices, 0);

		for (int x = 0; x < downIndices.Length - 1; x++) {
			int leftValue = downIndices [x];
			int rightValue = downIndices [x + 2];
			reverseDownIndices [x] = rightValue;
			reverseDownIndices [x + 2] = leftValue;
			x += 2;
		}

		reverseDownIndices.CopyTo (downIndices, 0);

		int[] middleIndices = new int[verticesPositionCount * 6];
		int middleIndicesIndex = 0;

		for (int x = 0; x < verticesPositionCount; x++) {
			int leftIndex = x;
			int rightIndex = x + 1;

			if (x == verticesPositionCount - 1) {
				rightIndex = 0;
			}

			middleIndices [middleIndicesIndex] = rightIndex;
			middleIndicesIndex++;
			middleIndices [middleIndicesIndex] = leftIndex;
			middleIndicesIndex++;

			if (x == verticesPositionCount - 1) {
				middleIndices [middleIndicesIndex] = (verticesPositionCount * 2) - 1;
			} else {
				middleIndices [middleIndicesIndex] = rightIndex + verticesPositionCount - 1;
			}
			middleIndicesIndex++;

			middleIndices [middleIndicesIndex] = leftIndex + verticesPositionCount;
			middleIndicesIndex++;

			if (x == verticesPositionCount - 1) {
				middleIndices [middleIndicesIndex] = verticesPositionCount;
			} else {
				middleIndices [middleIndicesIndex] = rightIndex + verticesPositionCount;
			}

			middleIndicesIndex++;

			if (x == verticesPositionCount - 1) {
				middleIndices [middleIndicesIndex] = 0;
			} else {
				middleIndices [middleIndicesIndex] = rightIndex;
			}

			middleIndicesIndex++;
		}

		List<Vector2> topVertices2D = new List<Vector2> ();

		for (int i = 0; i < topCorners.Length; i++) {
			topVertices2D.Add (new Vector2 (topCorners [i].x, topCorners [i].z));
		}

		GKC_Triangulator_Utils topTr = new GKC_Triangulator_Utils (topVertices2D);
		int[] topdIndices = topTr.Triangulate ();

		for (int i = 0; i < topdIndices.Length; i++) {
			topdIndices [i] += verticesPositionCount;
		}

		int[] combinedIndices = new int[downIndices.Length + topdIndices.Length + middleIndices.Length];

		downIndices.CopyTo (combinedIndices, 0);
		middleIndices.CopyTo (combinedIndices, downIndices.Length);
		topdIndices.CopyTo (combinedIndices, downIndices.Length + middleIndices.Length);

		mapPart3dMesh.triangles = combinedIndices;

		mapPart3dMesh.RecalculateNormals ();

		mapManager.addMapPart3dMeshToFloorParent (mapPart3dGameObject, gameObject);
	}

	public void setMapManager (mapCreator currentMapManager)
	{
		mapManager = currentMapManager;

		updateComponent ();
	}

	public void setMapPartParent (Transform currentMapPartParent)
	{
		mapPartParent = currentMapPartParent;

		updateComponent ();
	}

	public void setMapPartBuildingIndex (int newIndex)
	{
		mapPartBuildingIndex = newIndex;

		updateComponent ();
	}

	public void setMapPartFlooorIndex (int newIndex)
	{
		mapPartFloorIndex = newIndex;

		updateComponent ();
	}

	public void setMapPartIndex (int newIndex)
	{
		mapPartIndex = newIndex;

		updateComponent ();
	}

	public void setRandomMapPartColor ()
	{
		mapPartMaterialColor = new Vector4 (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 1);

		updateComponent ();
	}

	public void updateComponent ()
	{
		GKC_Utils.updateComponent (this);

		GKC_Utils.updateDirtyScene ("Update Map Tile Renderer " + gameObject.name, gameObject);
	}

	//draw every floor position and a line between floors
	void OnDrawGizmos ()
	{
		if (!showGizmo) {
			return;
		}

		if (GKC_Utils.isCurrentSelectionActiveGameObject (gameObject)) {
			DrawGizmos ();
		}
	}

	void OnDrawGizmosSelected ()
	{
		DrawGizmos ();
	}

	//draw the pivot and the final positions of every door
	void DrawGizmos ()
	{
		if (showGizmo && mapManager.showMapPartsGizmo && !Application.isPlaying) {
			center = Vector3.zero;

			for (int i = 0; i < verticesPosition.Count; i++) {
				if (verticesPosition [i] != null) {
					if (i + 1 < verticesPosition.Count) {
						if (verticesPosition [i + 1] != null) {
							if (mapManager.useSameLineColor) {
								Gizmos.color = mapManager.mapLinesColor;
							} else {
								Gizmos.color = mapLinesColor;
							}

							Gizmos.DrawLine (verticesPosition [i].position, verticesPosition [i + 1].position);
						}
					}

					if (i == verticesPosition.Count - 1) {
						if (verticesPosition [0] != null) {
							if (mapManager.useSameLineColor) {
								Gizmos.color = mapManager.mapLinesColor;
							} else {
								Gizmos.color = mapLinesColor;
							}
							Gizmos.DrawLine (verticesPosition [i].position, verticesPosition [0].position);
						}
					}
					center += verticesPosition [i].position;
				} 
			}

			center /= verticesPosition.Count;

			if (showEnabledTrigger && mapManager.showMapPartEnabledTrigger) {
				for (int i = 0; i < eventTriggerList.Count; i++) {
					if (eventTriggerList [i] != null) {
						Gizmos.color = mapManager.enabledTriggerGizmoColor;
						Gizmos.DrawCube (eventTriggerList [i].transform.position, eventTriggerList [i].transform.localScale);

						Gizmos.color = Color.yellow;
						Gizmos.DrawLine (eventTriggerList [i].transform.position, center);
					}
				}
			}

			if (mapManager.showMapPartsTextGizmo && textMeshList.Count > 0) {
				for (int i = 0; i < textMeshList.Count; i++) {
					if (textMeshList [i] != null) {
						Gizmos.color = Color.red;
						Gizmos.DrawSphere (textMeshList [i].transform.position, 0.1f);

						Gizmos.color = Color.blue;
						Gizmos.DrawLine (textMeshList [i].transform.position, center);
					}
				}
			}

			if (generate3dMapPartMesh && (mapManager.generate3dMeshesShowGizmo || generate3dMeshesShowGizmo)) {
				current3dOffset = Vector3.up * mapPart3dHeight;
				Gizmos.color = Color.white;
				Gizmos.DrawLine (center + mapPart3dOffset, center + mapPart3dOffset + current3dOffset);
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere (center + mapPart3dOffset, 0.2f);
				Gizmos.DrawSphere (center + mapPart3dOffset + current3dOffset, 0.2f);

				for (int i = 0; i < verticesPosition.Count; i++) {
					if (verticesPosition [i] != null) {
						if (i + 1 < verticesPosition.Count) {
							if (verticesPosition [i + 1] != null) {
								if (mapManager.useSameLineColor) {
									Gizmos.color = mapManager.mapLinesColor;
								} else {
									Gizmos.color = mapLinesColor;
								}
								Gizmos.DrawLine (verticesPosition [i].position + current3dOffset, verticesPosition [i + 1].position + current3dOffset);
							}
						}
						if (i == verticesPosition.Count - 1) {
							if (verticesPosition [0] != null) {
								if (mapManager.useSameLineColor) {
									Gizmos.color = mapManager.mapLinesColor;
								} else {
									Gizmos.color = mapLinesColor;
								}
								Gizmos.DrawLine (verticesPosition [i].position + current3dOffset, verticesPosition [0].position + current3dOffset);
							}
						}
					} 
					Gizmos.DrawLine (verticesPosition [i].position, verticesPosition [i].position + current3dOffset);
				}
			}

			Gizmos.color = mapPartMaterialColor;
			Gizmos.DrawCube (center, cubeGizmoScale);
		}
	}
}