using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TelloLib;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using calculation;

public class TelloController : SingletonMonoBehaviour<TelloController> {

	private static bool isLoaded = false;

	private TelloVideoTexture telloVideoTexture;

	double ry_count = 0.0;
	double rx_count = 0.0;
	double lx_count = 0.0;
	double count = 0.0;
	int ry_flag = 0;
	int rx_flag = 0;
	int lx_flag = 0;
	double x = 0.0;
	double y = 0.0;
	double z = 0.0;

    private TextureSender textureSender;

	// FlipType is used for the various flips supported by the Tello.
	public enum FlipType
	{

		// FlipFront flips forward.
		FlipFront = 0,

		// FlipLeft flips left.
		FlipLeft = 1,

		// FlipBack flips backwards.
		FlipBack = 2,

		// FlipRight flips to the right.
		FlipRight = 3,

		// FlipForwardLeft flips forwards and to the left.
		FlipForwardLeft = 4,

		// FlipBackLeft flips backwards and to the left.
		FlipBackLeft = 5,

		// FlipBackRight flips backwards and to the right.
		FlipBackRight = 6,

		// FlipForwardRight flips forewards and to the right.
		FlipForwardRight = 7,
	};

	// VideoBitRate is used to set the bit rate for the streaming video returned by the Tello.
	public enum VideoBitRate
	{
		// VideoBitRateAuto sets the bitrate for streaming video to auto-adjust.
		VideoBitRateAuto = 0,

		// VideoBitRate1M sets the bitrate for streaming video to 1 Mb/s.
		VideoBitRate1M = 1,

		// VideoBitRate15M sets the bitrate for streaming video to 1.5 Mb/s
		VideoBitRate15M = 2,

		// VideoBitRate2M sets the bitrate for streaming video to 2 Mb/s.
		VideoBitRate2M = 3,

		// VideoBitRate3M sets the bitrate for streaming video to 3 Mb/s.
		VideoBitRate3M = 4,

		// VideoBitRate4M sets the bitrate for streaming video to 4 Mb/s.
		VideoBitRate4M = 5,

	};

	override protected void Awake()
	{
		if (!isLoaded) {
			DontDestroyOnLoad(this.gameObject);
			isLoaded = true;
		}
		base.Awake();

		Tello.onConnection += Tello_onConnection;
		Tello.onUpdate += Tello_onUpdate;
		Tello.onVideoData += Tello_onVideoData;

		if (telloVideoTexture == null)
			telloVideoTexture = FindObjectOfType<TelloVideoTexture>();

	}

	private void OnEnable()
	{
		if (telloVideoTexture == null)
			telloVideoTexture = FindObjectOfType<TelloVideoTexture>();
	}

	private void Start()
	{
		if (telloVideoTexture == null)
			telloVideoTexture = FindObjectOfType<TelloVideoTexture>();

		Tello.startConnecting();

        if (textureSender == null)
            textureSender = FindObjectOfType<TextureSender>();
	}

	void OnApplicationQuit()
	{
		Tello.stopConnecting();
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.T)) {
			Tello.takeOff();
		} else if (Input.GetKeyDown(KeyCode.L)) {
			Tello.land();
		}

        double a = textureSender.estimater.position[0];
        double b = textureSender.estimater.position[2];

        double c = textureSender.estimater.rotation[1];
        Debug.Log(String.Format("{0},{1}, {2}", a, b, c));

        /* ���v���W�F�N�g���ŐV�����V�[�����쐬���A
         * �V�K�ɍ쐬����3D�I�u�W�F�N�g(Plane)�ɂ��̃X�N���v�g�ƁuTelloVideoTexture.cs�v�uTextureSender.cs(����,AR�}�[�J�[���m�p)�v���A�^�b�`
         * ���s��T�L�[�������ė����A���̌�K�L�[�������ƂȂ��������ƈ������ɐi�ݑ�����
         * ��L�́uTextureSender.cs�v�������Ă��Ȃ��Ă��o�O�͕ς��Ȃ�
         * �i�ݑ����Ă���Œ���L�L�[�������ƒ����͍s�����Ƃ��ł��A���̌�Ɋm�F�p�̃��O�uaaaaaaa...�v�Ɓubbbbbbbbbbbb...�v���\�������
         */
        
        if (Input.GetKeyDown(KeyCode.K))
		{
			ry_count = 0.0;
			rx_count = 0.0;
			lx_count = 0.0;
			ry_flag = 1;
			rx_flag = 1;
			lx_flag = 0;

            // �O
            // �����Ńh���[�����ړ����Ȃ���΂Ȃ�Ȃ��������v�Z���Ă���
            // �����̐��l���Ԉ�����l�ɂȂ��Ă���킯�ł͂Ȃ�
            // ���̈����̒l����x = 50, y=-100�ƂȂ�
            x = Calculation.X(50.0d, 0.0d, 90.0d);
			// ��
			y = Calculation.Y(50.0d, 0.0d, 90.0d);
			// ��]
			z = 90.0;

            Debug.Log("x:" + x.ToString() + " y:" + y.ToString());
		}

        //�@�ړ����I�����������n�߂�O��50�t���[�����x�Ȃɂ����Ȃ��悤�ɂ���
        if (ry_flag == 2 && rx_flag == 2 && lx_flag == 0)
        {
			count++;
			if(count >= 50)
            {
				lx_flag = 1;
				count = 0;
			}
        }

        // ��]���I�������l�����ɖ߂�
		else if (ry_flag == 2 && rx_flag == 2 && lx_flag == 2)
        {
			//Tello.land();

			ry_count = 0.0;
			rx_count = 0.0;
			lx_count = 0.0;
			ry_flag = 0;
			rx_flag = 0;
			lx_flag = 0;

			// �O
			//x = Calculation.X(150.0d, 45.0d, 45.0d);
			// ��
			//y = Calculation.Y(150.0d, 45.0d, 45.0d);
			// ����
		    //z = 45.0;

			// ����:�J���������ʂ��Ƃ炦���ꍇ���ʂȓ����������

			//Debug.Log("x:" + x.ToString() + " y:" + y.ToString());
		}

		float lx = 0f;
		float ly = 0f;
		float rx = 0f;
		float ry = 0f;

        // ���t���[������ �ړ�������
		if ((int)x > 0 && ry_flag == 1) {
			ry = 1f;
			ry_count += 1;
			if ((int)x <= ry_count){
				ry_flag = 2;
                Debug.Log("aaaaaaaaaaaaaaaaaaaaa");
			}
		}
		if ((int)x < 0 && ry_flag == 1) {
			ry = -0.5f;
			ry_count += 0.5;
			if (-1*(int)x <= ry_count)
			{
				ry_flag = 2;
			}
		}

		if ((int)y > 0 && rx_flag == 1) {
			rx = 0.5f;
			rx_count += 0.5;
			if ((int)y <= rx_count)
			{
				rx_flag = 2;
			}
		}
		if ((int)y < 0 && rx_flag == 1) {
			rx = -1f;
			rx_count += 1;
			if (-1*(int)y <= rx_count)
			{
				rx_flag = 2;
                Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
			}
		}

        // ���t���[������ ��]������

        // ����:���Ԃ�����΃J���������ʂ��Ƃ炦���ꍇ���ʂȓ����������
        // �������͂��Ă��Ȃ�
        if (0 < (int)z && (int)z <= 180 && lx_flag == 1) {
			lx = 0.5f;
			lx_count += 0.5;
			if ((int)z <= lx_count)
			{
				lx_flag = 2;
			}
		}
		if (180 < (int)z && (int)z < 360 && lx_flag == 1) {
			lx = -0.5f;
			lx_count += 0.5;
			if (360.0-(int)z <= lx_count)
			{
				lx_flag = 2;
			}
		}

		//if (Input.GetKey(KeyCode.UpArrow)) {
		//	ry = 1;
		//}
		//if (Input.GetKey(KeyCode.DownArrow)) {
		//	ry = -1;
		//}
		//if (Input.GetKey(KeyCode.RightArrow)) {
		//	rx = 1;
		//}
		//if (Input.GetKey(KeyCode.LeftArrow)) {
		//	rx = -1;
		//}
		//if (Input.GetKey(KeyCode.W)) {
		//	ly = 1;
		//}
		//if (Input.GetKey(KeyCode.S)) {
		//	ly = -1;
		//}
		//if (Input.GetKey(KeyCode.D)) {
		//	lx = 1;
		//}
		//if (Input.GetKey(KeyCode.A)) {
		//	lx = -1;
		//}
		Tello.controllerState.setAxis(lx, ly, rx, ry);

	}

	private void Tello_onUpdate(int cmdId)
	{
		//throw new System.NotImplementedException();
		//Debug.Log("Tello_onUpdate : " + Tello.state);
	}

	private void Tello_onConnection(Tello.ConnectionState newState)
	{
		//throw new System.NotImplementedException();
		//Debug.Log("Tello_onConnection : " + newState);
		if (newState == Tello.ConnectionState.Connected) {
            Tello.queryAttAngle();
            Tello.setMaxHeight(50);

			Tello.setPicVidMode(1); // 0: picture, 1: video
			Tello.setVideoBitRate((int)VideoBitRate.VideoBitRateAuto);
			//Tello.setEV(0);
			Tello.requestIframe();
		}
	}

	private void Tello_onVideoData(byte[] data)
	{
		//Debug.Log("Tello_onVideoData: " + data.Length);
		if (telloVideoTexture != null)
			telloVideoTexture.PutVideoData(data);
	}

}
