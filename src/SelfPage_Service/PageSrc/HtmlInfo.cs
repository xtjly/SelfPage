using SelfPage_Service.PageInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelfPage_Service.PageSrc
{
    public static class HtmlInfo
    {
        internal static string GetHtmlPageInfo(ResPageInfo resPageInfo, List<string> realyGroups)
        {
            string html = $@"
<!DOCTYPE html>
<html lang='zh-cn'>
<head>
    <meta charset='utf-8' />
    <title>SelfPage接口文档</title>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap@3.3.7/dist/css/bootstrap.min.css' integrity='sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u' crossorigin='anonymous'>
    <!-- SelfPageCSS -->
    <style>
        body {{
            margin: 0;
            padding: 0;
            background: #000000;
            overflow-x: hidden;
        }}
        .SelfPage-Control {{
            color:black;
            background:white;
            font-size:30px;
            margin-top:10px;
        }}
    </style>
</head>
<body>
    <canvas id='canvas'style='position:absolute;left:0;top:0;z-index:-1;'></canvas>
    <div class='form-inline'><select id='SelfPage-Group' class='form-control' ></select></div>
    <div id='SelfPage-Controllers'>{Div_Controllers_Get(resPageInfo)}</div>

    
    <script src='https://cdn.jsdelivr.net/npm/bootstrap@3.3.7/dist/js/bootstrap.min.js' integrity='sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa' crossorigin='anonymous'></script>
    <script src='https://cdn.staticfile.org/jquery/2.2.4/jquery.min.js'></script>
    <script>
        const PARTICLES_PER_FIREWORK = 150;
        const FIREWORK_CHANCE = 0.02;
        const BASE_PARTICLE_SPEED = 0.6;
        const FIREWORK_LIFESPAN = 600;
        const PARTICLE_INITIAL_SPEED = 4.5;
        const GRAVITY = 9.8;
        const canvas = document.getElementById('canvas');
        const ctx = canvas.getContext('2d');
        let particles = [];
        let disableAutoFireworks = false;
        let resetDisable = 0;
        let loop = () => {{
            if (!disableAutoFireworks && Math.random() < FIREWORK_CHANCE) {{
                createFirework();
            }}
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            particles.forEach((particle, i) => {{
                particle.animate();
                particle.render();
                if (particle.y > canvas.height
                    || particle.x < 0
                    || particle.x > canvas.width
                    || particle.alpha <= 0
                ) {{
                    particles.splice(i, 1);
                }}
            }});
            requestAnimationFrame(loop);
        }};
        let createFirework = (
            x = Math.random() * canvas.width,
            y = Math.random() * canvas.height
        ) => {{
            let speed = (Math.random() * 2) + BASE_PARTICLE_SPEED;
            let maxSpeed = speed;
            let red = ~~(Math.random() * 255);
            let green = ~~(Math.random() * 255);
            let blue = ~~(Math.random() * 255);
            red = (red < 150 ? red + 150 : red);
            green = (green < 150 ? green + 150 : green);
            blue = (blue < 150 ? blue + 150 : blue);
            for (let i = 0; i < PARTICLES_PER_FIREWORK; i++) {{
                let particle = new Particle(x, y, red, green, blue, speed);
                particles.push(particle);
                maxSpeed = (speed > maxSpeed ? speed : maxSpeed);
            }}
            for (let i = 0; i < 40; i++) {{
                let particle = new Particle(x, y, red, green, blue, maxSpeed, true);
                particles.push(particle);
            }}
        }};
        class Particle {{
            constructor(
                x = 0,
                y = 0,
                red = ~~(Math.random() * 255),
                green = ~~(Math.random() * 255),
                blue = ~~(Math.random() * 255),
                speed,
                isFixedSpeed
            ) {{
                this.x = x;
                this.y = y;
                this.red = red;
                this.green = green;
                this.blue = blue;
                this.alpha = 0.05;
                this.radius = 1 + Math.random();
                this.angle = Math.random() * 360;
                this.speed = (Math.random() * speed) + 0.1;
                this.velocityX = Math.cos(this.angle) * this.speed;
                this.velocityY = Math.sin(this.angle) * this.speed;
                this.startTime = (new Date()).getTime();
                this.duration = Math.random() * 300 + FIREWORK_LIFESPAN;
                this.currentDiration = 0;
                this.dampening = 30;
                this.colour = this.getColour();

                if (isFixedSpeed) {{
                    this.speed = speed;
                    this.velocityY = Math.sin(this.angle) * this.speed;
                    this.velocityX = Math.cos(this.angle) * this.speed;
                }}
                this.initialVelocityX = this.velocityX;
                this.initialVelocityY = this.velocityY;
            }}
            animate() {{
                this.currentDuration = (new Date()).getTime() - this.startTime;
                if (this.currentDuration <= 200) {{
                    this.x += this.initialVelocityX * PARTICLE_INITIAL_SPEED;
                    this.y += this.initialVelocityY * PARTICLE_INITIAL_SPEED;
                    this.alpha += 0.01;
                    this.colour = this.getColour(240, 240, 240, 0.9);
                }} else {{
                    this.x += this.velocityX;
                    this.y += this.velocityY;
                    this.colour = this.getColour(this.red, this.green, this.blue, 0.4 + (Math.random() * 0.3));
                }}
                this.velocityY += GRAVITY / 1000;
                if (this.currentDuration >= this.duration) {{
                    this.velocityX -= this.velocityX / this.dampening;
                    this.velocityY -= this.velocityY / this.dampening;
                }}
                if (this.currentDuration >= this.duration + this.duration / 1.1) {{
                    this.alpha -= 0.02;
                    this.colour = this.getColour();
                }} else {{
                    if (this.alpha < 1) {{
                        this.alpha += 0.03;
                    }}
                }}
            }}
            render() {{
                ctx.beginPath();
                ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2, true);
                ctx.lineWidth = this.lineWidth;
                ctx.fillStyle = this.colour;
                ctx.shadowBlur = 8;
                ctx.shadowColor = this.getColour(this.red + 150, this.green + 150, this.blue + 150, 1);
                ctx.fill();
            }}
            getColour(red, green, blue, alpha) {{
                return `rgba(${{red || this.red}}, ${{green || this.green}}, ${{blue || this.blue}}, ${{alpha || this.alpha}})`;
            }}
        }}
        let updateCanvasSize = () => {{
            canvas.width = window.innerWidth;
            canvas.height = window.innerHeight;
        }};
        updateCanvasSize();
        $(window).resize(updateCanvasSize);
        $(canvas).on('click', (e) => {{
            createFirework(e.clientX, e.clientY);
            disableAutoFireworks = true;
            clearTimeout(resetDisable);
            resetDisable = setTimeout(() => {{
                disableAutoFireworks = false;
            }}, 5000);
        }});
        loop();
    </script>
    <script>
        {Select_Group_Get(resPageInfo, realyGroups)}
        
    </script>
</body>
</html>
";

            return html;
        }

        /// <summary>
        /// 完成控制器信息的加载
        /// </summary>
        /// <param name="resPageInfo"></param>
        /// <returns></returns>
        private static string Div_Controllers_Get(ResPageInfo resPageInfo)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            resPageInfo.Controllers.ForEach(control =>
            {
                sb.Append($@" <div id='SelfPage-Control-{i}' class='SelfPage-Control'>{control.ControllerName}<br/>{control.Discribetion}</div> ");
                i++;
            });
            return sb.ToString();
        }

        /// <summary>
        /// 分组下拉框select加载分组option，及完成Js function
        /// </summary>
        /// <param name="realyGroups"></param>
        /// <returns></returns>
        private static string Select_Group_Get(ResPageInfo resPageInfo, List<string> realyGroups)
        {
            StringBuilder sb = new StringBuilder();
            realyGroups.ForEach(group =>
            {
                if (group.Equals(resPageInfo.GroupName, StringComparison.CurrentCultureIgnoreCase))
                {
                    sb.Append($@" $('#SelfPage-Group').append('<option selected values=""{group}"">{group}</option>'); ");
                }
                else
                {
                    sb.Append($@" $('#SelfPage-Group').append('<option values=""{group}"">{group}</option>'); ");
                }
            });
            sb.Append($@"
                $('#SelfPage-Group').change(function(){{
                    window.location=window.origin+'/selfpage/'+$(this).val(); 
                }});
            ");
            return sb.ToString();
        }



    }
}
