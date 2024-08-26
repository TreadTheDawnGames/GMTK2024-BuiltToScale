using Godot;
using System;

public partial class CameraZoomer : Camera2D
{
	Control Deck;
    Control ResetButton;
	Vector2 Home;

    int slowScrollTimer = 50;
    int slowScrollTimerMax = 25;
    bool timerActive = false;

    public bool GameOver = false;
    public float maxCamHeight { get; private set; }
    public float loseHeight { get; private set; }
    public bool Zooming { get; private set; } = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Home = GlobalPosition;
		Deck = GetNode<Control>("Deck");
		ResetButton = GetNode<Control>("CanvasLayer/ResetGameButton");
        maxCamHeight = -460;
        slowScrollTimer = slowScrollTimerMax;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {   
        
        if ((Input.IsActionJustPressed("ZoomOut")|| Input.IsActionJustPressed("SlowScrollDown"))&&!Zooming)
        {
            absMouseWheeleDisplacement = (int)maxCamHeight;
        }
        if (Input.IsActionPressed("SlowScrollDown") && !GameOver)
        {
                ScrollableZoom();
            slowScrollTimer--;
            if (slowScrollTimer < 0)
            {
                timerActive = true;
                if (absMouseWheeleDisplacement < Home.Y && absMouseWheeleDisplacement >= maxCamHeight)
                    absMouseWheeleDisplacement += 10;
            }
            
            else
            {
                timerActive = false;
            }
        }
        

        if (Input.IsActionPressed("FullZoomOut") && !timerActive)
        {
            FullZoomOut(0.25f);
        }
        if (Input.IsActionPressed("ZoomOut") && !GameOver && !timerActive)
        {
            ScrollableZoom();
        }
        else if (!GameOver &&!Input.IsActionPressed("FullZoomOut")&&!timerActive && !Input.IsActionPressed("SlowScrollDown"))
        {
            Zooming = false;
            initializePosition = false;
            absMouseWheeleDisplacement = 0;

            Zoom = Zoom.Lerp(new Vector2(1f, 1f), 0.05f);
            Deck.Modulate = Deck.Modulate.Lerp(new Color(1, 1, 1, 1), 0.25f);
            if (!GameOver)
            {
                ResetButton.Modulate = ResetButton.Modulate.Lerp(new Color(1, 1, 1, 1), 0.25f);
                ResetButton.Scale = ResetButton.Scale.Lerp(new Vector2(1, 1), 0.25f);
            }
            Deck.ProcessMode = ProcessModeEnum.Always;
            GlobalPosition = GlobalPosition.Lerp(new Vector2(GlobalPosition.X, maxCamHeight), 0.1f);


            if (GameManager.Instance.Rufus != null && GameManager.Instance.Rufus.GlobalPosition.Y < maxCamHeight)
            {
                maxCamHeight = GameManager.Instance.Rufus.GlobalPosition.Y;
                loseHeight = GlobalPosition.Y + 1080 / 2 + 35;
            }
            
        }
        if (Input.IsActionJustReleased("SlowScrollDown"))
        {
            slowScrollTimer = slowScrollTimerMax;
            timerActive = false;

        }
    }

    bool initializePosition = false;
    int absMouseWheeleDisplacement = 0;

    Vector2 workingPosition = new Vector2(0,0);
    void ScrollableZoom()
    {
        Zooming = true;
        Zoom = Zoom.Lerp(new Vector2(0.5f, 0.5f), 0.05f);
        Deck.Modulate = Deck.Modulate.Lerp(new Color(1, 1, 1, 0), 0.25f);
        if (!GameOver)
        {

            ResetButton.Modulate = ResetButton.Modulate.Lerp(new Color(1, 1, 1, 0), 0.25f);
            ResetButton.Scale = ResetButton.Scale.Lerp(new Vector2(0, 0), 0.25f);
        }


        GD.Print("Displacement: " + absMouseWheeleDisplacement);

        if(absMouseWheeleDisplacement >= maxCamHeight)
        {
            workingPosition.Y =  absMouseWheeleDisplacement;
        }



        GlobalPosition = GlobalPosition.Lerp(new Vector2(GlobalPosition.X, workingPosition.Y), 0.1f);

    }

    public void FullZoomOut(float speed)
    {
        var goToMultiplier = 0.5f;
        if (maxCamHeight >= -1000)
            goToMultiplier = 0.75f;
        //LimitTop = (int)deck.GlobalPosition.Y;

        //deck.Hide();
        Zooming = true;

        Deck.Modulate = Deck.Modulate.Lerp(new Color(1, 1, 1, 0), 0.25f);
        if (!GameOver)
        {

            ResetButton.Modulate = ResetButton.Modulate.Lerp(new Color(1, 1, 1, 0), 0.25f);
            ResetButton.Scale = ResetButton.Scale.Lerp(new Vector2(0, 0), 0.25f);
        }

        Deck.ProcessMode = ProcessModeEnum.Disabled;
        GlobalPosition = GlobalPosition.Lerp(new Vector2(Home.X, (maxCamHeight * goToMultiplier)), speed);

        var zoomPower = Mathf.Clamp((1080) / -maxCamHeight, 0f, 0.75f);

        if (zoomPower > 0)
        {

            Zoom = Zoom.Lerp(new Vector2(zoomPower, zoomPower), speed);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            InputEventMouseButton emb = (InputEventMouseButton)@event;
            if (emb.IsPressed())
            {
                if (emb.ButtonIndex == MouseButton.WheelUp)
                {
                    if(absMouseWheeleDisplacement>=maxCamHeight)
                        absMouseWheeleDisplacement-=75;
                    if(absMouseWheeleDisplacement<maxCamHeight)
                    {
                        absMouseWheeleDisplacement = (int)maxCamHeight;
                    }
                }
                if (emb.ButtonIndex == MouseButton.WheelDown)
                {
                    if(absMouseWheeleDisplacement<Home.Y)
                        absMouseWheeleDisplacement+=75;
                    if (absMouseWheeleDisplacement >=Home.Y)
                    {
                        absMouseWheeleDisplacement = (int)Home.Y;
                    }
                }
            }
        }
    }
}
