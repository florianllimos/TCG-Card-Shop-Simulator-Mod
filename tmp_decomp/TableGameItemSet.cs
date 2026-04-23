using UnityEngine;

public class TableGameItemSet : MonoBehaviour
{
	public MeshRenderer m_PlayMatMesh;

	public MeshRenderer m_DeckBoxMesh;

	public MeshRenderer m_ComicBookMesh;

	public void RandomizeSetup()
	{
		int num = Random.Range(0, 3);
		EItemType randomItemTypeFromCategory = InventoryBase.GetRandomItemTypeFromCategory(EItemCategory.Deckbox, unlockedOnly: true);
		int num2 = Random.Range(0, 100);
		if (randomItemTypeFromCategory != EItemType.None && num2 < 70)
		{
			ItemMeshData itemMeshData = InventoryBase.GetItemMeshData(randomItemTypeFromCategory);
			m_DeckBoxMesh.material = itemMeshData.material;
			Vector3 localPosition = m_DeckBoxMesh.transform.localPosition;
			switch (num)
			{
			case 0:
				localPosition.x = 0.45f;
				break;
			case 1:
				localPosition.x = -0.45f;
				break;
			}
			localPosition.z = Random.Range(0.2f, 0.45f);
			Quaternion localRotation = m_DeckBoxMesh.transform.localRotation;
			Vector3 eulerAngles = localRotation.eulerAngles;
			eulerAngles.y = Random.Range(0, 360);
			localRotation.eulerAngles = eulerAngles;
			m_DeckBoxMesh.transform.localPosition = localPosition;
			m_DeckBoxMesh.transform.localRotation = localRotation;
			m_DeckBoxMesh.gameObject.SetActive(value: true);
		}
		else
		{
			m_DeckBoxMesh.gameObject.SetActive(value: false);
		}
		EItemType randomItemTypeFromCategory2 = InventoryBase.GetRandomItemTypeFromCategory(EItemCategory.Manga, unlockedOnly: true);
		num2 = Random.Range(0, 100);
		if (randomItemTypeFromCategory2 != EItemType.None && num2 < 20)
		{
			ItemMeshData itemMeshData2 = InventoryBase.GetItemMeshData(randomItemTypeFromCategory2);
			m_ComicBookMesh.material = itemMeshData2.material;
			Vector3 localPosition2 = m_ComicBookMesh.transform.localPosition;
			switch (num)
			{
			case 0:
				localPosition2.x = -0.45f;
				break;
			case 1:
				localPosition2.x = 0.45f;
				break;
			}
			localPosition2.z = Random.Range(0.2f, 0.45f);
			Quaternion localRotation2 = m_ComicBookMesh.transform.localRotation;
			Vector3 eulerAngles2 = localRotation2.eulerAngles;
			eulerAngles2.y = Random.Range(0, 360);
			localRotation2.eulerAngles = eulerAngles2;
			m_ComicBookMesh.transform.localPosition = localPosition2;
			m_ComicBookMesh.transform.localRotation = localRotation2;
			m_ComicBookMesh.gameObject.SetActive(value: true);
		}
		else
		{
			m_ComicBookMesh.gameObject.SetActive(value: false);
		}
		EItemType randomItemTypeFromCategory3 = InventoryBase.GetRandomItemTypeFromCategory(EItemCategory.Playmat, unlockedOnly: true);
		num2 = Random.Range(0, 100);
		if (randomItemTypeFromCategory3 != EItemType.None && num2 < 95)
		{
			ItemMeshData itemMeshData3 = InventoryBase.GetItemMeshData(randomItemTypeFromCategory3);
			m_PlayMatMesh.material = itemMeshData3.material;
			m_PlayMatMesh.gameObject.SetActive(value: true);
		}
		else
		{
			m_PlayMatMesh.gameObject.SetActive(value: false);
		}
	}
}
