bgDark = Color(63, 63, 116)
bgLight = Color(215, 123, 186)
borderDark = Color(34, 32, 52)
borderLight = Color(118, 66, 138)

app.command.NewFile {
    ui = false,
    width = 32,
    height = 32,
    colorMode = ColorMode.RGB,
    fromClipboard = false
}

app.useTool {
    tool = "paint_bucket",
    color = bgDark,
    points = {Point(0, 0)}
}

app.useTool {
    tool = "rectangle",
    color = borderDark,
    brush = Brush(1),
    points = {Point(0, 0), Point(31, 31)}
}

app.command.NewLayer {}

app.useTool {
    tool = "paint_bucket",
    color = bgLight,
    points = {Point(0, 0)}
}

app.useTool {
    tool = "rectangle",
    color = borderLight,
    brush = Brush(1),
    points = {Point(0, 0), Point(31, 31)}
}

app.useTool {
    tool = "filled_rectangle",
    color = Color(0, 0, 0, 0),
    points = {Point(0, 0), Point(15, 15)}
}

app.useTool {
    tool = "filled_rectangle",
    color = Color(0, 0, 0, 0),
    points = {Point(16, 16), Point(31, 31)}
}

-- app.command.NewLayer {}

-- local name = string.upper("a")
-- local x = 0
-- local y = 0
-- for char in name:gmatch "." do
--     local token = font[char]
--     print(token.name, token.codepoint)
-- end
