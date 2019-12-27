using SelfPage_Service.PageInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelfPage_Service.PageSrc
{
    public static class HtmlInfo
    {
        /// <summary>
        /// 返回接口文档信息
        /// </summary>
        /// <param name="resPageInfo"></param>
        /// <param name="realyGroups"></param>
        /// <param name="pageInfo"></param>
        /// <returns></returns>
        internal static string GetHtmlPageInfo(ResPageInfo resPageInfo, List<string> realyGroups, SelfPageInfo pageInfo)
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
            background:#f5f5f5cc;
            border:5px solid #00ffe7e6;
            border-radius:20px;
            font-size:30px;
            margin:10px;
            padding:10px;
        }}
        .SelfPage-Action{{
            background: #d9edf775;
            border: 3px solid #337ab7c4;
            border-radius: 10px;
            margin: 10px;
            padding: 10px;
            color: #337ab7;
        }}
        .SelfPage-Action-Span{{
            float:right;
            margin-right:10px;
            border: 3px solid #8a6d3b;
            padding:0 5px;
            color:#795548;
        }}
        .SelfPage-Action-Body{{
            background:#8bc34ac2;
            margin:10px;
            padding:10px;
            color:black;
            font-size:20px;
        }}
    </style>
</head>
<body>
    <canvas id='canvas'style='position:absolute;left:0;top:0;z-index:-1;'></canvas>
    <div class='form-inline' style='text-align:center;'><lable style='font-size:50px;font-weight:bold;color:white;'>SelfPage<lable/><select id='SelfPage-Group' class='form-control' style='padding: 0 3%; margin:0px 10px;'></select></div>
    <div id='SelfPage-Controllers'>{Div_Controllers_Get(resPageInfo, pageInfo)}</div>

    
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
        {Select_Group_Get(resPageInfo, realyGroups, pageInfo)}
        {Div_Actions_Show(resPageInfo)}        
        {Div_MethodBody_Show(resPageInfo, pageInfo)}
        

    </script>
</body>
</html>
";

            return html;
        }

        /// <summary>
        /// 控制Action下的body是否显示，及为执行按钮设置监听 JS
        /// </summary>
        /// <param name="resPageInfo"></param>
        /// <returns></returns>
        private static string Div_MethodBody_Show(ResPageInfo resPageInfo, SelfPageInfo pageInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($@" $(""div[id^='SelfPage-Bodys-Action']"").hide(); ");
            int i = 1;
            resPageInfo.Controllers.ForEach(control =>
            {
                int n = 1;
                control.Actions.ForEach(action =>
                {
                    sb.Append($@" 
                        $('#SelfPage-Action-Span-{n}-Control-{i}').click(function(){{
                            if($('#SelfPage-Bodys-Action-{n}-Control-{i}').is(':visible')){{ 
                                $('#SelfPage-Bodys-Action-{n}-Control-{i}').hide();
                                $('#SelfPage-Action-Span-{n}-Control-{i}').html('打开测试');
                            }}else{{
                                $('#SelfPage-Bodys-Action-{n}-Control-{i}').show();
                                $('#SelfPage-Action-Span-{n}-Control-{i}').html('关闭测试');
                            }}
                        }}); 
                    ");
                    sb.Append($@" 
                        $('#SelfPage-ExcuteButton-Action-{n}-Control-{i}').click(function(){{
                            $.ajax({{
                                type:'{(action.RequestType == RequestType.HttpGet ? "get" : "post")}',
                                dataType:'json',
	                            contentType: 'application/json',
	                            url: window.origin + '{action.RequestPath}',
                                {(pageInfo.AddAuthorizationHeader ? $@"headers:{{'authorization':$('#SelfPage-Excute-Action-{n}-Control-{i}-Authorization').val()}}," : "")}
                                success:function(res){{
                                    $('#SelfPage-ExcuteReturn-Action-{n}-Control-{i}').html(JSON.stringify(res));
                                }},
                                error:function(error){{
                                    $('#SelfPage-ExcuteReturn-Action-{n}-Control-{i}').html(JSON.stringify(error));
                                }},
                            }});
                        }});
                    ");
                    n++;
                });
                i++;
            });
            return sb.ToString();
        }

        /// <summary>
        /// 控制控制器下的Actions是否显示 JS
        /// </summary>
        /// <param name="resPageInfo"></param>
        /// <returns></returns>
        private static string Div_Actions_Show(ResPageInfo resPageInfo)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            sb.Append($@" $(""div[id^='SelfPage-Actions-Control']"").hide(); ");
            resPageInfo.Controllers.ForEach(control =>
            {
                sb.Append($@" 
                        $('#SelfPage-Control-Span-{i}').click(function(){{
                            if($('#SelfPage-Actions-Control-{i}').is(':visible')){{ 
                                $('#SelfPage-Control-Span-{i}').attr('class','glyphicon glyphicon-chevron-right');
                                $('#SelfPage-Actions-Control-{i}').hide();
                            }}else{{
                                $('#SelfPage-Control-Span-{i}').attr('class','glyphicon glyphicon-chevron-down');
                                $('#SelfPage-Actions-Control-{i}').show();
                            }}
                        }}); 
                ");
                i++;
            });
            return sb.ToString();
        }

        /// <summary>
        /// 完成控制器信息的加载
        /// </summary>
        /// <param name="resPageInfo"></param>
        /// <returns></returns>
        private static string Div_Controllers_Get(ResPageInfo resPageInfo, SelfPageInfo pageInfo)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            resPageInfo.Controllers.ForEach(control =>
            {
                sb.Append($@" <div id='SelfPage-Control-{i}' class='SelfPage-Control'>{control.ControllerName}<span style='margin:0 50px;'>{control.Discribetion}</span><span id='SelfPage-Control-Span-{i}' style='float:right;margin-right:10px;' class='glyphicon glyphicon-chevron-right' aria-hidden='true'></span> ");
                sb.Append($@" <div id='SelfPage-Actions-Control-{i}'> ");
                int n = 1;
                control.Actions.ForEach(action =>
                {
                    sb.Append($@" <div id='SelfPage-Action-{n}-Control-{i}' class='SelfPage-Action'>{GetActionRequestTypeSpan(action.RequestType)}{action.RequestPath}{GetActionDescribtionSpan(action.DescribeTion)}<button class='SelfPage-Action-Span'><span id='SelfPage-Action-Span-{n}-Control-{i}' >打开测试</span></button> ");
                    sb.Append($@" <div id='SelfPage-Bodys-Action-{n}-Control-{i}' class='SelfPage-Action-Body'> ");

                    if (pageInfo.AddAuthorizationHeader)
                    {
                        sb.Append($@" <lable>Authorization：</lable><input id='SelfPage-Excute-Action-{n}-Control-{i}-Authorization'><br/> ");
                    }
                    sb.Append($@" <button  id='SelfPage-ExcuteButton-Action-{n}-Control-{i}'>执行测试</button> ");
                    sb.Append($@" <div>预计返回结果：</div><div>{action.ReturnJsonStr}</div> ");
                    sb.Append($@" <div>实际返回结果：</div><div id='SelfPage-ExcuteReturn-Action-{n}-Control-{i}'></div> ");

                    sb.Append($@"</div>"); //end bodys
                    sb.Append($@" </div> "); //end action
                    n++;
                });
                sb.Append($@"</div>"); //end control-actions
                sb.Append($@"</div>"); //end control
                i++;
            });
            return sb.ToString();
        }

        /// <summary>
        /// 获取action的描述控件
        /// </summary>
        /// <param name="describeTion"></param>
        /// <returns></returns>
        private static object GetActionDescribtionSpan(string describeTion)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($@" <span style='margin:0 50px;'>{describeTion}</span> ");
            return sb.ToString();
        }

        /// <summary>
        /// 获取action的请求方式控件
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        private static string GetActionRequestTypeSpan(RequestType requestType)
        {
            StringBuilder sb = new StringBuilder();
            if (requestType == RequestType.HttpGet)
            {
                sb.Append($@" <span style='color:green;margin-right:50px;'>{requestType}</span> ");
            }
            else
            {
                sb.Append($@" <span style='color:red;margin-right:50px;'>{requestType}</span> ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 分组下拉框select加载分组option，及完成Js function
        /// </summary>
        /// <param name="realyGroups"></param>
        /// <returns></returns>
        private static string Select_Group_Get(ResPageInfo resPageInfo, List<string> realyGroups, SelfPageInfo pageInfo)
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
                    window.location=window.origin+'{pageInfo.EndPointPath}/'+$(this).val(); 
                }});
            ");
            return sb.ToString();
        }



    }
}
