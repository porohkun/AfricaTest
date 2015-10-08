# Исходные данные: #

* набор арта для одного материка (фон, материк, объекты на карте, животное на карте из 4х частей)

* превью 

* превью с дефолтным расположением камеры

# Целевое разрешение 1024x768. #

 Арт пропорционален целевому разрешению.


Решать задачу можно используя плагины tk2d, EasyTouch, DOTween, либо встроенными средствами Unity.


На выходе должен получиться проект на актуальной версии Unity (5.2 в данный момент), который нужно скинуть для проверки (zip архивом, например). Обычно нужно исключить из проекта различные кеши Unity (папки Library, temp и т.д.). Должно хватить папок Assets и ProjectSettings. 

Но перед отправкой нужно проверить, что проект корректно откроется в таком виде.

# Необходимо: #

1. Собрать материк по превью из имеющегося арта. Целью является корректная сборка карты в соответствии с превью, корректная настройка камеры, спрайтов и атласов. Животное должно быть собрано с расчетом на анимацию.

2. Сделать зум и скролл. Зум происходит с помощью жестов Pinch In, Pinch Out. Есть минимальное и максимальное приближение - по умолчанию камера на максимальном удалении. Также в определенных пределах карту можно скроллить. При скролле должна быть плавная доводка (когда отпускаем и какое-то время карта скроллится по инерции), а также выход за ограничители скролла должен происходить с оттягиванием (то есть когда можно оттягивать камеру на определенную величину за границу, но при отпускании она возвращается в границы). Целью является максимально плавное и корректное поведение зума и скролла, лишенное странного поведения, багов и ошибок.